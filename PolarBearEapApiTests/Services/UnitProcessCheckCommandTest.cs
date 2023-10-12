using Microsoft.Extensions.Logging;
using Moq;
using PolarBearEapApi.Commons;
using PolarBearEapApi.Models;
using PolarBearEapApi.Services;

namespace PolarBearEapApiTests
{
    public class UnitProcessCheckCommandTest
    {
        private const string MES_RETURN_DISPLAY = "WF002:Routing Error, Goto <TE020>";

        /** 
         * 測試執行成功
         * Given: Mes回傳OK
         * Then: 回傳 "{\"Result\":\"OK\"}", ErrorMessage = null
         */
        [Fact]
        public void TestSuccess()
        {
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.UNIT_PROCESS_CHECK(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns("{\"Result\":\"OK\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":null,\"BindInfo\":null}");

            var command = new UnitProcessCheckCommand(null!, mockMesService.Object);

            MesCommandResponse response = command.Execute(MockMesCommandRequest());
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
        public void TestFail()
        {
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.UNIT_PROCESS_CHECK(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .Returns("{\"Result\":\"NG\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":\"" + MES_RETURN_DISPLAY + "\",\"BindInfo\":null}");

            var command = new UnitProcessCheckCommand(null!, mockMesService.Object);

            MesCommandResponse response = command.Execute(MockMesCommandRequest());
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
        public void TestMesThrowException()
        {
            var mockLogger = new Mock<ILogger<UnitProcessCheckCommand>>();
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.UNIT_PROCESS_CHECK(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();

            var command = new UnitProcessCheckCommand(mockLogger.Object, mockMesService.Object);

            MesCommandResponse response = command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(ErrorCodeEnum.CallMesServiceException.ToString(), response.ErrorMessage);
            Assert.Equal("{\"Result\":\"NG\"}", response.OpResponseInfo);
        }

        private static MesCommandRequest MockMesCommandRequest()
        {
            MesCommandRequest request = new MesCommandRequest();
            request.Hwd = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57";
            request.SerializeData = "{\"Hwd\": \"36bd2cd3-3c94-4d53-a494-79eab5d34e9f\",\"Indicator\": \"ADD_RECORD\",\"SerializeData\": \"{\\\"LineCode\\\":\\\"E08-1FT-01\\\",\\\"SectionCode\\\":\\\"TE060\\\",\\\"StationCode\\\":72607,\\\"OPCategory\\\":\\\"UNIT_PROCESS_COMMIT\\\",\\\"OPRequestInfo\\\":{\\\"SN\\\":\\\"H2C336500010VC6RX\\\"},\\\"OPResponseInfo\\\":{}}\"}";
            return request;
        }
    }
}
