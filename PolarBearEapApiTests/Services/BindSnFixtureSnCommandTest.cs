using Microsoft.Extensions.Logging;
using Moq;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Services;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApiUnitTests.Services
{
    public class BindSnFixtureSnCommandTest
    {
        private const string MES_RETURN_DISPLAY = "ID003:The login account does not have permission to operate this workstation";

        /** 
       * 測試執行成功
       * Given: Mes回傳OK
       * Then: 回傳 "{\"Result\":\"OK\"}", ErrorMessage = null
       */
        [Fact]
        public async Task TestSuccess()
        {
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.BIND_FIXTURE_BY_SN_FIXTURE(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("{\"Result\":\"OK\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":null,\"BindInfo\":null}");

            var command = new BindSnFixtureSnCommand(mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
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
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.BIND_FIXTURE_BY_SN_FIXTURE(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("{\"Result\":\"NG\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":\"" + MES_RETURN_DISPLAY + "\",\"BindInfo\":null}");

            var command = new BindSnFixtureSnCommand(mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
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
            var mockTokenService = new Mock<ITokenRepository>();
            var mockLogger = new Mock<ILogger<BindCommand>>();
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.BIND_FIXTURE_BY_SN_FIXTURE(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();

            var command = new BindSnFixtureSnCommand(mockMesService.Object);

            var caughtException = await Assert.ThrowsAsync<EapException>(() => command.Execute(MockMesCommandRequest()));
            Assert.Contains(ErrorCodeEnum.CallMesServiceException.ToString(), caughtException.Message);
        }

        private static MesCommandRequest MockMesCommandRequest()
        {
            MesCommandRequest request = new MesCommandRequest();
            request.Hwd = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57";
            request.SerializeData = "{\"LineCode\":\"E09-1FT-01A\",\"SectionCode\":\"VT_SEC_0\",\"StationCode\":300900,\"OPCategory\":\"BIND_SN_FIXTURESN\",\"OPRequestInfo\":{\"SN\":\"PPPZH002JQH3\",\"FIXTURE_SN\":\"PPPZH002JQH3001\"},\"OPResponseInfo\":{}}";
            return request;
        }
    }
}
