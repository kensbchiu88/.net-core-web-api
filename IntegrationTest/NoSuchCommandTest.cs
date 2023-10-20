using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.PublicApi.Models;
using System.Text;

namespace IntegrationTest
{
    public class NoSuchCommandTest
    {
        private HttpClient _httpClient;

        private const string HWD = "36bd2cd3-3c94-4d53-a494-79eab5d34e9f";
        private const string INDICATOR = "ADD_ATTR";
        private const string LINE_CODE = "E08-1FT-01";
        private const string SECTION_CODE = "T04A-STATION81";
        private const int STATION_CODE = 72607;
        private const string OP_CATEGORY = "NO_SUCH_COMMAND";
        private const string SN = "J21GYM0028100000US";

        public NoSuchCommandTest() 
        {
            var webApplicationFactory = new WebApplicationFactory<Program>();
            _httpClient = webApplicationFactory.CreateDefaultClient();
        }

        [Fact]
        public async Task TestNoSuchCommand() 
        {
            var requestModel = new ApiRequest() { };
            var endPoint = "/api/v1/EapApi";
            var requestBody = FakeApiRequest();
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endPoint, content);

            var result = await response.Content.ReadAsStringAsync();
            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Equal(HWD, JsonUtil.GetCaseSensitiveParameter(result, "Hwd"));
            Assert.Equal("NG", JsonUtil.GetParameter(JsonUtil.GetParameter(result, "SerializeData"), "OPResponseInfo.Result"));
            Assert.Equal(ErrorCodeEnum.NoSuchCommand.ToString(), JsonUtil.GetCaseSensitiveParameter(result, "Display"));

        }

        private static string FakeApiRequest()
        {
            JObject opRequestInfo = new JObject();
            opRequestInfo["SN"] = SN;

            JObject serializeData = new JObject();
            serializeData["LineCode"] = LINE_CODE;
            serializeData["SectionCode"] = SECTION_CODE;
            serializeData["StationCode"] = STATION_CODE.ToString();
            serializeData["OPCategory"] = OP_CATEGORY;
            serializeData["OPRequestInfo"] = opRequestInfo;
            serializeData["OPResponseInfo"] = new JObject();

            JObject apiRquest = new JObject();
            apiRquest["Hwd"] = HWD;
            apiRquest["Indicator"] = INDICATOR;
            apiRquest["SerializeData"] = serializeData.ToString(Formatting.None);

            return apiRquest.ToString().Replace(Environment.NewLine, string.Empty);
        }
    }

}
