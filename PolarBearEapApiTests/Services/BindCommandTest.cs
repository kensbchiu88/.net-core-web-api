using Microsoft.Extensions.Logging;
using Moq;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Services;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApiTests
{
    public class BindCommandTest
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
            var mockTokenService = new Mock<ITokenRepository>();
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.CHECK_SECTION_PERMISSION(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync ("{\"Result\":\"OK\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":null,\"BindInfo\":null}");
            mockTokenService.Setup(service => service.GetTokenInfo(It.IsAny<string>())).ReturnsAsync(MockTokenInfo);
            mockTokenService.Setup(service => service.BindMachine(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Verifiable();

            var command = new BindCommand(mockTokenService.Object, null, mockMesService.Object);

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
            var mockTokenService = new Mock<ITokenRepository>();
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.CHECK_SECTION_PERMISSION(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("{\"Result\":\"NG\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":\"" + MES_RETURN_DISPLAY + "\",\"BindInfo\":null}");
            mockTokenService.Setup(service => service.GetTokenInfo(It.IsAny<string>())).ReturnsAsync(MockTokenInfo);

            var command = new BindCommand(mockTokenService.Object, null, mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(MES_RETURN_DISPLAY, response.ErrorMessage);
            Assert.Equal("{\"Result\":\"NG\"}", response.OpResponseInfo);
        }

        /** 
         * 測試MES回傳Exception
         * Given: MES丟出Exception
         * Then: 回傳 Result:"NG", ErrorMessage = CallMesServiceException
         */
        [Fact]
        public async Task TestMesThrowException()
        {
            var mockTokenService = new Mock<ITokenRepository>();
            var mockLogger = new Mock<ILogger<BindCommand>>();
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.CHECK_SECTION_PERMISSION(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();
            mockTokenService.Setup(service => service.GetTokenInfo(It.IsAny<string>())).ReturnsAsync(MockTokenInfo);

            var command = new BindCommand(mockTokenService.Object, mockLogger.Object, mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(ErrorCodeEnum.CallMesServiceException.ToString(), response.ErrorMessage);
            Assert.Equal("{\"Result\":\"NG\"}", response.OpResponseInfo);
        }

        private static MesCommandRequest MockMesCommandRequest()
        {
            MesCommandRequest request = new MesCommandRequest();
            request.Hwd = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57";
            request.SerializeData = "{\"Hwd\": \"36bd2cd3-3c94-4d53-a494-79eab5d34e9f\",\"Indicator\": \"QUERY_RECORD\",\"SerializeData\": \"{\\\"LineCode\\\":\\\"E08-1FT-01\\\",\\\"SectionCode\\\":\\\"T04A-STATION81\\\",\\\"StationCode\\\":72607,\\\"OPCategory\\\":\\\"GET_SN_BY_SN_FIXTURE\\\",\\\"OPRequestInfo\\\":{\\\"REF_VALUE\\\":\\\"UC030-0001-0001\\\",\\\"REF_TYPE\\\":\\\"SN_FIXTURE\\\"},\\\"OPResponseInfo\\\":{}}\"}";
            return request;
        }

        private static TokenInfo MockTokenInfo() 
        {
            TokenInfo tokenInfo = new TokenInfo
            {
                Id = "fake_id",
                username = "username"
            };
            return tokenInfo;
        }
    }
}
