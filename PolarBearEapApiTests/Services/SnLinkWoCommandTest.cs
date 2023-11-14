using Moq;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Services;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApiUnitTests.Services
{
    public class SnLinkWoCommandTest
    {
        private const string MES_RETURN_DISPLAY = "WIP004:SN has been bound to the work order";
        private const string HWD = "3279682D-31D0-4DBB-82C9-7EC7CCEA12D6";
        private const string USERNAME = "username";

        /** 
         * 測試執行成功
         * Given: Mes回傳OK
         * Then: 回傳 "{\"Result\":\"OK\"}", ErrorMessage = null
         */
        [Fact]
        public async Task TestSuccess()
        {
            var fakekMesService = new Mock<IMesService>();
            var fakekTokenService = new Mock<ITokenRepository>();

            fakekMesService.Setup(service => service.SN_LINK_WO(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("{\"Result\":\"OK\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":null,\"BindInfo\":null}");

            fakekTokenService.Setup(service => service.GetTokenInfo(It.IsAny<string>())).ReturnsAsync(FakeTokenInfo());


            var command = new SnLinkWoCommand(fakekMesService.Object, fakekTokenService.Object);

            MesCommandResponse response = await command.Execute(FakeMesCommandRequest());
            Assert.NotNull(response);
            Assert.Null(response.ErrorMessage);
            Assert.Equal("{\"Result\":\"OK\"}", response.OpResponseInfo);
        }

        /** 
         * 測試執行失敗
         * Given: Mes回傳NG
         * Then: 回傳 "{\"Result\":\"NG\"}", ErrorMessage = MES回傳的Display欄位
         */
        [Fact]
        public async Task TestFail()
        {
            var fakekMesService = new Mock<IMesService>();
            var fakekTokenService = new Mock<ITokenRepository>();

            fakekMesService.Setup(service => service.SN_LINK_WO(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("{\"Result\":\"NG\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":\"" + MES_RETURN_DISPLAY + "\",\"BindInfo\":null}");

            fakekTokenService.Setup(service => service.GetTokenInfo(It.IsAny<string>())).ReturnsAsync(FakeTokenInfo());

            var command = new SnLinkWoCommand(fakekMesService.Object, fakekTokenService.Object);

            MesCommandResponse response = await command.Execute(FakeMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(MES_RETURN_DISPLAY, response.ErrorMessage);
            Assert.Equal("{\"Result\":\"NG\"}", response.OpResponseInfo);
        }

        /** 
         * 測試MES回傳Exception
         * Given: MES丟出Exception
         * Then: throw EapException, and Message = CallMesServiceException
         */
        [Fact]
        public async Task TestMesThrowException()
        {
            var fakekMesService = new Mock<IMesService>();
            var fakekTokenService = new Mock<ITokenRepository>();

            fakekMesService.Setup(service => service.SN_LINK_WO(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws<Exception>();

            fakekTokenService.Setup(service => service.GetTokenInfo(It.IsAny<string>())).ReturnsAsync(FakeTokenInfo());

            var command = new SnLinkWoCommand(fakekMesService.Object, fakekTokenService.Object);

            var caughtException = await Assert.ThrowsAsync<EapException>(() => command.Execute(FakeMesCommandRequest()));
            Assert.Contains(ErrorCodeEnum.CallMesServiceException.ToString(), caughtException.Message);
        }

        /** 
         * 測試input沒有SN資訊
         * Given: 沒有SN資訊的input
         * Then: throw EapException and Message contains JsonFieldRequire
         */
        [Fact]
        public async Task TestInputWithoutSN()
        {
            var command = new SnLinkWoCommand(null, null);
            var caughtException = await Assert.ThrowsAsync<EapException>(() => command.Execute(MockMesCommandRequestWithoutSN()));
            Assert.Contains("JsonFieldRequire", caughtException.Message);
        }

        /** 
         * 測試input沒有FIXTURE_SN資訊
         * Given: 沒有FIXTURE_SN資訊的input
         * Then: throw EapException and Message contains JsonFieldRequire
         */
        [Fact]
        public async Task TestInputWithoutFIXTURE_SN()
        {
            var command = new SnLinkWoCommand(null, null);
            var caughtException = await Assert.ThrowsAsync<EapException>(() => command.Execute(MockMesCommandRequestWithoutWo()));
            Assert.Contains("JsonFieldRequire", caughtException.Message);
        }

        private static MesCommandRequest FakeMesCommandRequest()
        {
            MesCommandRequest request = new MesCommandRequest();
            request.Hwd = HWD;
            request.SerializeData = "{\"LineCode\":\"E08-1FT-01\",\"SectionCode\":\"STATION118\",\"StationCode\":1180001,\"OPCategory\":\"SN_LINK_WO\",\"OPRequestInfo\":{\"SN\":\"J44341600021FQWRK\",\"WO\":\"214774070153\"},\"OPResponseInfo\":{}}";
            return request;
        }

        private static MesCommandRequest MockMesCommandRequestWithoutSN()
        {
            MesCommandRequest request = new MesCommandRequest();
            request.Hwd = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57";
            request.SerializeData = "{\"LineCode\":\"E08-1FT-01\",\"SectionCode\":\"STATION118\",\"StationCode\":1180001,\"OPCategory\":\"SN_LINK_WO\",\"OPRequestInfo\":{\"WO\":\"214774070153\"},\"OPResponseInfo\":{}}";
            return request;
        }

        private static MesCommandRequest MockMesCommandRequestWithoutWo()
        {
            MesCommandRequest request = new MesCommandRequest();
            request.Hwd = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57";
            request.SerializeData = "{\"LineCode\":\"E08-1FT-01\",\"SectionCode\":\"STATION118\",\"StationCode\":1180001,\"OPCategory\":\"SN_LINK_WO\",\"OPRequestInfo\":{\"SN\":\"J44341600021FQWRK\"},\"OPResponseInfo\":{}}";
            return request;
        }

        private static TokenInfo FakeTokenInfo()
        {
            return new TokenInfo 
            {
                Id = HWD,
                username = USERNAME
            };
        }
    }
}
