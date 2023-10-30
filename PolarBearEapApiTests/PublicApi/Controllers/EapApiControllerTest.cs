using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.ApplicationCore.Services;
using PolarBearEapApi.PublicApi.Controllers;
using PolarBearEapApi.PublicApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Sdk;

namespace PolarBearEapApiUnitTests.PublicApi.Controllers
{
    public class EapApiControllerTest
    {
        private const string HWD = "36bd2cd3-3c94-4d53-a494-79eab5d34e9f";
        private const string INDICATOR = "ADD_RECORD";
        private const string LINE_CODE = "E08-1FT-01";
        private const string SECTION_CODE = "T04A-STATION81";
        private const int STATION_CODE = 72607;
        private const string OP_CATEGORY = "BIND";
        private const string SERVER_VERSION = "ServerVersion";
        private const string OP_RESPONSE_INFO = "{\"Result\":\"NG\"}";
        private const string ERROR_MESSAGE = "Test Error Message";


        /** 
         * 呼叫Api
         * Given: Fake的ApiRequest, MesCommandResponse
         * Then: 1. Hwd與input相同
         *       2. Indicator與input相同
         *       3. OPResponseInfo.Result = NG (與fake MesCommandResponse相同)
         *       4. Display 與fake MesCommandResponse相同
         */
        [Fact]
        public async Task TestApi()
        {
            var mockCommandFactoryService = new Mock<IMesCommandFactory<IMesCommand>>();
            var mockCommand = new Mock<IMesCommand>();


            mockCommandFactoryService.Setup(service => service.Get(It.IsAny<string>())).Returns(mockCommand.Object);
            mockCommand.Setup(service => service.Execute(It.IsAny<MesCommandRequest>())).ReturnsAsync(FakeMesCommandResponse());

            var controller = new EapApiController(null, mockCommandFactoryService.Object);

            ApiResponse response = await controller.Api(FakeApiRequest());
            Assert.NotNull(response);
            Assert.Equal(HWD, response.Hwd);
            Assert.Equal(INDICATOR, response.Indicator);
            Assert.Equal("NG", JsonUtil.GetParameter(response.SerializeData, "OPResponseInfo.Result"));
            Assert.Equal(ERROR_MESSAGE, response.Display);
        }

        //{"Hwd": "B36C976F-23BB-49E3-B567-4D2449EFA7F6","Indicator": "ADD_RECORD","SerializeData": "{\"LineCode\":\"E08-1FT-01\",\"SectionCode\":\"O-01\",\"StationCode\":12346,\"OPCategory\":\"BIND\",\"OPRequestInfo\":{\"ServerVersion\":\"v1.01\"},\"OPResponseInfo\":{}}"}
        private static ApiRequest FakeApiRequest()
        {
            JObject opRequestInfo = new JObject();
            opRequestInfo["ServerVersion"] = SERVER_VERSION;

            JObject serializeData = new JObject();
            serializeData["LineCode"] = LINE_CODE;
            serializeData["SectionCode"] = SECTION_CODE;
            serializeData["StationCode"] = STATION_CODE;
            serializeData["OPCategory"] = OP_CATEGORY;
            serializeData["OPRequestInfo"] = opRequestInfo;
            serializeData["OPResponseInfo"] = new JObject();

            return new ApiRequest
            {
                Hwd = HWD,
                Indicator = INDICATOR,
                SerializeData = serializeData.ToString(Formatting.None)
            };
        }

        private static MesCommandResponse FakeMesCommandResponse()
        {
            return new MesCommandResponse
            {
                OpResponseInfo = OP_RESPONSE_INFO,
                ErrorMessage = ERROR_MESSAGE
            };
        }
    }
}
