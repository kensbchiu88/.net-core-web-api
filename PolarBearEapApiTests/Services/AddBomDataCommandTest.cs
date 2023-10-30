using Microsoft.Extensions.Logging;
using Moq;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Services;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApiTests
{
    public class AddBomDataCommandTest
    {

        private const string MES_RETURN_DISPLAY = "AUTO001:MES station is not defined";
        public AddBomDataCommandTest() 
        { 
        
        }

        /** 
         * 測試執行成功
         * Given: Mes回傳OK
         * Then: 回傳 "{\"Result\":\"OK\"}", ErrorMessage = null
         */
        [Fact]
        public async Task TestSuccess()
        {
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.ADD_BOM_DATA(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("{\"Result\":\"OK\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":null,\"BindInfo\":null}");


            var command = new AddBomDataCommand(null, mockMesService.Object);

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

            mockMesService.Setup(service => service.ADD_BOM_DATA(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync("{\"Result\":\"NG\",\"ResultCoded\":\"\",\"MessageCode\":null,\"Display\":\"" + MES_RETURN_DISPLAY + "\",\"BindInfo\":null}");


            var command = new AddBomDataCommand(null, mockMesService.Object);

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

            var mockLogger = new Mock<ILogger<AddBomDataCommand>>();
            var mockMesService = new Mock<IMesService>();

            mockMesService.Setup(service => service.ADD_BOM_DATA(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Throws<Exception>();
            
            var command = new AddBomDataCommand(mockLogger.Object, mockMesService.Object);

            MesCommandResponse response = await command.Execute(MockMesCommandRequest());
            Assert.NotNull(response);
            Assert.Equal(ErrorCodeEnum.CallMesServiceException.ToString(), response.ErrorMessage);
            Assert.Equal("{\"Result\":\"NG\"}", response.OpResponseInfo);
        }


        private static MesCommandRequest MockMesCommandRequest()
        {
            MesCommandRequest request = new MesCommandRequest();
            request.Hwd = "EE2DDC7D-5EF5-4B4E-A66F-961823865A57";
            request.SerializeData = "{\"LineCode\":\"E08-1FT-01\",\"SectionCode\":\"TE010\",\"StationCode\":12344,\"OPCategory\":\"ADD_BOM_DATA\",\"OPRequestInfo\":{\"SN\":\"H2C3365000W0VC6RX\",\"RAW_SN\":\"J213363036B1WLK1X\"},\"OPResponseInfo\":{}}";
            return request;
        }
    }
}
