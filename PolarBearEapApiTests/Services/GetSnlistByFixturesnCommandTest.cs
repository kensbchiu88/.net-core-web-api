using Microsoft.Extensions.Logging;
using Moq;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Services;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApiUnitTests.Services
{
    public class GetSnlistByFixturesnCommandTest
    {
        private const string SN = "sn1;sn2";
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

            mockMesService.Setup(service => service.GET_SNLIST_BY_FIXTURESN(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("{\"Result\":\"OK\",\"ResultCoded\":\"" + SN + "\",\"MessageCode\":null,\"Display\":null,\"BindInfo\":null}");

            var command = new GetSnlistByFixturesnCommand(mockMesService.Object);

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

            mockMesService.Setup(service => service.GET_SNLIST_BY_FIXTURESN(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("{\"Result\":\"NG\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":\"" + MES_RETURN_DISPLAY + "\",\"BindInfo\":null}");

            var command = new GetSnlistByFixturesnCommand(mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(MES_RETURN_DISPLAY, response.ErrorMessage);
            Assert.Equal("{\"SN\":\"\"}", response.OpResponseInfo);
        }

        /** 
         * 測試MES回傳Exception
         * Given: MES丟出Exception
         * Then: throw EapException and Message contains CallMesServiceException
         */
        [Fact]
        public async Task TestMesThrowException()
        {
            var mockLogger = new Mock<ILogger<GetSnBySnFixtureCommand>>();
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.GET_SNLIST_BY_FIXTURESN(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();

            var command = new GetSnlistByFixturesnCommand(mockMesService.Object);

            var caughtException = await Assert.ThrowsAsync<EapException>(() => command.Execute(MockMesCommandRequest()));
            Assert.Contains(ErrorCodeEnum.CallMesServiceException.ToString(), caughtException.Message);
        }

        private static MesCommandRequest MockMesCommandRequest()
        {
            MesCommandRequest request = new MesCommandRequest();
            request.Hwd = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57";
            request.SerializeData = "{\"LineCode\":\"E08-1FT-01\",\"SectionCode\":\"T04A-STATION432\",\"StationCode\":72617,\"OPCategory\":\"GET_SNLIST_BY_FIXTURESN\",\"OPRequestInfo\":{\"REF_VALUE\":\"OlytoBE2206081668\",\"REF_TYPE\":\"SN_FIXTURE\"},\"OPResponseInfo\":{}}";
            return request;
        }
    }
}
