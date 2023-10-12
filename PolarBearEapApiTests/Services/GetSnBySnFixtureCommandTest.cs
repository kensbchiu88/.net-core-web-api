using Microsoft.Extensions.Logging;
using Moq;
using PolarBearEapApi.Commons;
using PolarBearEapApi.Models;
using PolarBearEapApi.Services;

namespace PolarBearEapApiTests
{
    public class GetSnBySnFixtureCommandTest
    {
        private const string SN = "H2C336500020VC6RX";
        private const string MES_RETURN_DISPLAY = "AUTO001:MES station is not defined";

        /** 
         * 測試執行成功
         * Given: Mes回傳OK
         * Then: 回傳 "{\"SN\":\"H2C336500020VC6RX\"}", ErrorMessage = null
         */
        [Fact]
        public void TestSuccess() 
        {
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.GET_SN_BY_SN_FIXTURE(It.IsAny<string>()))
                .Returns("{\"Result\":\"OK\",\"ResultCoded\":\"" + SN + "\",\"MessageCode\":null,\"Display\":null,\"BindInfo\":null}");

            var command = new GetSnBySnFixtureCommand(null, mockMesService.Object);

            MesCommandResponse response = command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Null(response.ErrorMessage);
            Assert.Equal("{\"SN\":\"" + SN + "\"}", response.OpResponseInfo);
        }

        /** 
         * 測試執行失敗
         * Given: Mes回傳NG
         * Then: 回傳 "{\"SN\":\"\"}", ErrorMessage = MES回傳的Display欄位
         */
        [Fact] public void TestFail() 
        {
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.GET_SN_BY_SN_FIXTURE(It.IsAny<string>()))
                .Returns("{\"Result\":\"NG\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":\"" + MES_RETURN_DISPLAY + "\",\"BindInfo\":null}");

            var command = new GetSnBySnFixtureCommand(null, mockMesService.Object);

            MesCommandResponse response = command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(MES_RETURN_DISPLAY, response.ErrorMessage);
            Assert.Equal("{\"SN\":\"\"}", response.OpResponseInfo);
        }

        /** 
         * 測試MES回傳Exception
         * Given: MES丟出Exception
         * Then: 回傳 Result:"NG", ErrorMessage = CallMesServiceException
         */
        [Fact]
        public void TestMesThrowException()
        {
            var mockLogger = new Mock<ILogger<GetSnBySnFixtureCommand>>();
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.GET_SN_BY_SN_FIXTURE(It.IsAny<string>())).Throws<Exception>();

            var command = new GetSnBySnFixtureCommand(mockLogger.Object, mockMesService.Object);

            MesCommandResponse response = command.Execute(MockMesCommandRequest());
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
    }
}
