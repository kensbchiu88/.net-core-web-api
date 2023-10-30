using Microsoft.Extensions.Logging;
using Moq;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Services;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApiUnitTests.Services
{
    public class GetSnBySmtSnCommandTest
    {
        private const string SN = "H2C336500020VC6RX";
        private const string MES_RETURN_DISPLAY = "AUTO001:MES station is not defined";

        /** 
         * 測試執行成功
         * Given: Mes回傳OK
         * Then: 回傳 "{\"SN\":\"H2C336500020VC6RX\"}", ErrorMessage = null
         */
        [Fact]
        public async Task TestSuccess()
        {
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.GET_SN_BY_SMTSN(It.IsAny<string>()))
                .ReturnsAsync("{\"Result\":\"OK\",\"ResultCoded\":\"" + SN + "\",\"MessageCode\":null,\"Display\":null,\"BindInfo\":null}");

            var command = new GetSnBySmtSnCommand(null, mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Null(response.ErrorMessage);
            Assert.Equal("{\"SN\":\"" + SN + "\"}", response.OpResponseInfo);
        }

        /** 
         * 測試執行失敗
         * Given: Mes回傳NG
         * Then: 回傳 "{\"SN\":\"\"}", ErrorMessage = MES回傳的Display欄位
         */
        [Fact]
        public async Task TestFail()
        {
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.GET_SN_BY_SMTSN(It.IsAny<string>()))
                .ReturnsAsync("{\"Result\":\"NG\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":\"" + MES_RETURN_DISPLAY + "\",\"BindInfo\":null}");

            var command = new GetSnBySmtSnCommand(null, mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(MES_RETURN_DISPLAY, response.ErrorMessage);
            Assert.Equal("{\"SN\":\"\"}", response.OpResponseInfo);
        }

        /** 
         * 測試MES回傳Exception
         * Given: MES丟出Exception
         * Then: throw EapException且message contains CallMesServiceException
         */
        [Fact]
        public async Task TestMesThrowException()
        {
            var mockLogger = new Mock<ILogger<GetSnBySmtSnCommand>>();
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.GET_SN_BY_SMTSN(It.IsAny<string>())).Throws<Exception>();

            var command = new GetSnBySmtSnCommand(mockLogger.Object, mockMesService.Object);

            var caughtException = await Assert.ThrowsAsync<EapException>(() => command.Execute(MockMesCommandRequest()));
            Assert.Contains(ErrorCodeEnum.CallMesServiceException.ToString(), caughtException.Message);
        }

        /** 
         * 測試input沒有OPRequestInfo.REF_VALUE
         * Given: input沒有OPRequestInfo.REF_VALUE
         * Then: throw EapException且message contains CallMesServiceException
         */
        [Fact]
        public async Task TestNoRefValue()
        {
            var mockLogger = new Mock<ILogger<GetSnBySmtSnCommand>>();
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.GET_SN_BY_SMTSN(It.IsAny<string>())).Throws<Exception>();

            var command = new GetSnBySmtSnCommand(mockLogger.Object, mockMesService.Object);

            var caughtException = await Assert.ThrowsAsync<EapException>(() => command.Execute(MockMesCommandRequestWithoutRefValue()));
            Assert.Contains(ErrorCodeEnum.JsonFieldRequire.ToString(), caughtException.Message);
            Assert.Contains("OPRequestInfo.REF_VALUE is required", caughtException.Message);

        }

        private static MesCommandRequest MockMesCommandRequest()
        {
            MesCommandRequest request = new MesCommandRequest();
            request.Hwd = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57";
            request.SerializeData = "{\"LineCode\":\"E11-2FT-01B\",\"SectionCode\":\"STATION470\",\"StationCode\":16673,\"OPCategory\":\"GET_SN_BY_SMTSN\",\"OPRequestInfo\":{\"REF_VALUE\":\"G5Y2252007V1PP38C\"},\"OPResponseInfo\":{}}";
            return request;
        }

        private static MesCommandRequest MockMesCommandRequestWithoutRefValue()
        {
            MesCommandRequest request = new MesCommandRequest();
            request.Hwd = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57";
            request.SerializeData = "{\"LineCode\":\"E11-2FT-01B\",\"SectionCode\":\"STATION470\",\"StationCode\":16673,\"OPCategory\":\"GET_SN_BY_SMTSN\",\"OPRequestInfo\":{},\"OPResponseInfo\":{}}";
            return request;
        }
    }
}
