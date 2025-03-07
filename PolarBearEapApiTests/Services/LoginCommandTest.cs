﻿using Microsoft.Extensions.Logging;
using Moq;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Services;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Models;

namespace PolarBearEapApiTests
{
    public class LoginCommandTest
    {

        private const string fakeGuid = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57";

        public LoginCommandTest() 
        {
            //_command = new LoginCommand(_mockTokenService.Object, _mockLogger.Object, _mockMesService.Object);  
        }

        /** 
         * 測試帳號密碼正確
         * Given: 錯誤的帳號密碼
         * Then: 回傳 token ("{\"Hwd\":\"EE2DDC7D-5EF5-4B4E-A66F-961823865A57\"}"), ErrorMessage = null
         */
        [Fact]
        public async Task TestLoginSuccess()
        {
            var mockTokenService = new Mock<ITokenRepository>();
            var mockMesService = new Mock<IMesService>();

            // 設定模擬行為
            mockMesService.Setup(service => service.CHECK_OP_PASSWORD(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("{\"Result\":\"OK\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":null,\"BindInfo\":null}");
            mockTokenService.Setup(service => service.Create(It.IsAny<string>())).ReturnsAsync(fakeGuid);
            var command = new LoginCommand(mockTokenService.Object, null, mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Null(response.ErrorMessage);
            Assert.Equal("{\"Hwd\":\"" + fakeGuid + "\"}", response.OpResponseInfo);
        }

        /** 
         * 測試input沒有帳號資訊
         * Given: 沒有帳號資訊的input
         * Then: throw EapException and Message contains JsonFieldRequire
         */
        [Fact]
        public async Task TestInputWithoutUsername() 
        {
            var command = new LoginCommand(null, null, null);
            var caughtException = await Assert.ThrowsAsync<EapException>(() => command.Execute(MockMesCommandRequestWithoutUsername()));
            Assert.Contains("JsonFieldRequire", caughtException.Message);
        }

        /** 
         * 測試input沒有密碼資訊
         * Given: 沒有密碼資訊的input
         * Then: throw EapException and Message contains JsonFieldRequire
         */
        [Fact]
        public async Task TestInputWithoutPassword()
        {
            var command = new LoginCommand(null, null, null);
            var caughtException = await Assert.ThrowsAsync<EapException>(() => command.Execute(MockMesCommandRequestWithoutPassword()));
            Assert.Contains("JsonFieldRequire", caughtException.Message);
        }

        /** 
         * 測試帳號密碼錯誤
         * Given: 錯誤的帳號密碼
         * Then: 回傳 Hwd:"", ErrorMessage = LoginFail
         */
        [Fact]
        public async Task TestLoginFail()
        {
            var mockMesService = new Mock<IMesService>();
            // 設定模擬行為
            mockMesService.Setup(service => service.CHECK_OP_PASSWORD(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("{\"Result\":\"NG\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":\"ID002:The login account and password are incorrect.\",\"BindInfo\":null}");
            var command = new LoginCommand(null, null, mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(ErrorCodeEnum.LoginFail.ToString(), response.ErrorMessage);
            Assert.Equal("{\"Hwd\":\"\"}", response.OpResponseInfo);
        }

        /** 
         * 測試MES回傳Exception
         * Given: MES丟出Exception
         * Then: 回傳 Result:"NG", ErrorMessage = CallMesServiceException
         */
        [Fact]
        public async Task TestMesThrowException()
        {
            var mockLogger = new Mock<ILogger<LoginCommand>>();
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.CHECK_OP_PASSWORD(It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();
            var command = new LoginCommand(null, mockLogger.Object, mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(ErrorCodeEnum.CallMesServiceException.ToString(), response.ErrorMessage);
            Assert.Equal("{\"Result\":\"NG\"}", response.OpResponseInfo);
        }

        /** 
         * 測試產生Token發生Exception
         * Given: TokenService丟出Exception
         * Then: 回傳 Hwd:"", ErrorMessage = SaveTokenFail
         */
        [Fact]
        public async Task TestTokenThrowException()
        {
            var mockLogger = new Mock<ILogger<LoginCommand>>();
            var mockTokenService = new Mock<ITokenRepository>();
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.CHECK_OP_PASSWORD(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync("{\"Result\":\"OK\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":null,\"BindInfo\":null}");
            mockTokenService.Setup(service => service.Create(It.IsAny<string>())).Throws<Exception>();
            var command = new LoginCommand(mockTokenService.Object, mockLogger.Object, mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(ErrorCodeEnum.SaveTokenFail.ToString(), response.ErrorMessage);
            Assert.Equal("{\"Hwd\":\"\"}", response.OpResponseInfo);
        }

        private static MesCommandRequest MockMesCommandRequest()
        {
            MesCommandRequest request = new MesCommandRequest
            {
                Hwd = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57",
                SerializeData = "{\"LineCode\":\"\",\"SectionCode\":\"\",\"StationCode\":,\"OPCategory\":\"LOGIN\",\"OPRequestInfo\":{\"user\":\"fake_useranme\",\"pwd\":\"fake_password\"},\"OPResponseInfo\":{}}"
            };
            return request;
        }

        private static MesCommandRequest MockMesCommandRequestWithoutUsername()
        {
            MesCommandRequest request = new MesCommandRequest
            {
                Hwd = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57",
                SerializeData = "{\"LineCode\":\"\",\"SectionCode\":\"\",\"StationCode\":,\"OPCategory\":\"LOGIN\",\"OPRequestInfo\":{\"pwd\":\"fake_password\"},\"OPResponseInfo\":{}}"
            };
            return request;
        }

        private static MesCommandRequest MockMesCommandRequestWithoutPassword()
        {
            MesCommandRequest request = new MesCommandRequest
            {
                Hwd = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57",
                SerializeData = "{\"LineCode\":\"\",\"SectionCode\":\"\",\"StationCode\":,\"OPCategory\":\"LOGIN\",\"OPRequestInfo\":{\"user\":\"fake_useranme\"},\"OPResponseInfo\":{}}"
            };
            return request;
        }

    }
}
