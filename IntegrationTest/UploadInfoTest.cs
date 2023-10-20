using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using System.Text;
using PolarBearEapApi.Infra;
using Microsoft.Extensions.DependencyInjection;
using PolarBearEapApi.PublicApi.Models;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Extensions;

namespace IntegrationTest
{
    public class UploadInfoTest
    {
        private HttpClient _httpClient;
        private const string HWD = "36bd2cd3-3c94-4d53-a494-79eab5d34e9f";
        private const string UPLOAD_INDICATOR = "ADD_ATTR";
        private const string LINE_CODE = "E08-1FT-01";
        private const string SECTION_CODE = "T04A-STATION81";
        private const int STATION_CODE = 72607;
        private const string UPLOAD_OP_CATEGORY = "UPLOAD_INFOS";
        private const string SN = "J21GYM0028100000US";
        private const string FIELD_1 = "aaaaa";
        private const string FIELD_2 = "bbbbb";

        public UploadInfoTest() 
        {
            //var webApplicationFactory = new WebApplicationFactory<Program>();
            var webApplicationFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(host =>
                {
                    host.ConfigureServices(services =>
                    {
                        var dbContextDescriptor = services.SingleOrDefault(
                     d => d.ServiceType ==
                         typeof(DbContextOptions<UploadInfoDbContext>));

                        if (dbContextDescriptor != null)
                            services.Remove(dbContextDescriptor);


                        services.AddDbContext<UploadInfoDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("InMemoryEmployeeTest");
                        });
                    });
                });
            _httpClient = webApplicationFactory.CreateDefaultClient();
        }

        /** 
         * 測試執行成功
         * Given: "{\"Hwd\": \"36bd2cd3-3c94-4d53-a494-79eab5d34e9f\",\"Indicator\": \"ADD_ATTR\",\"SerializeData\": \"{\\\"LineCode\\\":\\\"E08-1FT-01\\\",\\\"SectionCode\\\":\\\"T04A-STATION81\\\",\\\"StationCode\\\":72607,\\\"OPCategory\\\":\\\"UPLOAD_INFOS\\\",\\\"OPRequestInfo\\\":{\\\"SN\\\":\\\"J21GYM0028100000US\\\",\\\"Gap1\\\":\\\"2.251\\\", \\\"A\\\":{\\\"A1\\\":\\\"a\\\",\\\"A2\\\":\\\"aa\\\", \\\"A3\\\":{\\\"AA3\\\":\\\"bb3\\\"}}},\\\"OPResponseInfo\\\":{}}\"}"
         * Then: Hwd與input時相同, 回傳 "{\"Result\":\"OK\"}", Display:null
         */
        [Fact]
        public async Task TestSuccess()
        {
            var requestModel = new ApiRequest() { };
            var endPoint = "/api/v1/EapApi";          
            var requestBody = FakeApiRequest();
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endPoint, content);

            var result = await response.Content.ReadAsStringAsync();
            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Equal(HWD, JsonUtil.GetCaseSensitiveParameter(result, "Hwd"));
            Assert.Equal("OK", JsonUtil.GetParameter(JsonUtil.GetParameter(result, "SerializeData"), "OPResponseInfo.Result"));
            Assert.Empty(JsonUtil.GetParameter(result, "Display"));
        }


        /** 
         * 帶入參數中缺少SN
         * Given: 帶入參數中缺少SN
         * Then: Hwd與input時相同, 回傳 "{\"Result\":\"NG\"}", Display:欄位有錯誤訊息
         */
        [Fact]
        public async Task TestWithoutSN()
        {
            var requestModel = new ApiRequest() { };
            var endPoint = "/api/v1/EapApi";            
            var requestBody = FakeApiRequestWithoutSN();
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endPoint, content);

            var result = await response.Content.ReadAsStringAsync();
            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Equal(HWD, JsonUtil.GetCaseSensitiveParameter(result, "Hwd"));
            Assert.Equal("NG", JsonUtil.GetParameter(JsonUtil.GetParameter(result, "SerializeData"), "OPResponseInfo.Result"));
            Assert.NotEmpty(JsonUtil.GetParameter(result, "Display"));
        }



        private static string FakeApiRequest()
        {
            JObject opRequestInfo = new JObject();
            opRequestInfo["SN"] = SN;
            opRequestInfo["FIELD_1"] = FIELD_1;
            opRequestInfo["FIELD_2"] = FIELD_2;
            
            JObject serializeData = new JObject();
            serializeData["LineCode"] = LINE_CODE;
            serializeData["SectionCode"] = SECTION_CODE;
            serializeData["StationCode"] = STATION_CODE.ToString();
            serializeData["OPCategory"] = UPLOAD_OP_CATEGORY;
            serializeData["OPRequestInfo"] = opRequestInfo;
            serializeData["OPResponseInfo"] = new JObject();

            JObject apiRquest = new JObject();
            apiRquest["Hwd"] = HWD;
            apiRquest["Indicator"] = UPLOAD_INDICATOR;
            apiRquest["SerializeData"] = serializeData.ToString(Formatting.None);

            return apiRquest.ToString().Replace(Environment.NewLine, string.Empty);
        }

        private static string FakeApiRequestWithoutSN()
        {
            JObject opRequestInfo = new JObject();
            opRequestInfo["FIELD_1"] = FIELD_1;
            opRequestInfo["FIELD_2"] = FIELD_2;

            JObject serializeData = new JObject();
            serializeData["LineCode"] = LINE_CODE;
            serializeData["SectionCode"] = SECTION_CODE;
            serializeData["StationCode"] = STATION_CODE.ToString();
            serializeData["OPCategory"] = UPLOAD_OP_CATEGORY;
            serializeData["OPRequestInfo"] = opRequestInfo;
            serializeData["OPResponseInfo"] = new JObject();

            JObject apiRquest = new JObject();
            apiRquest["Hwd"] = HWD;
            apiRquest["Indicator"] = UPLOAD_INDICATOR;
            apiRquest["SerializeData"] = serializeData.ToString(Formatting.None);

            return apiRquest.ToString().Replace(Environment.NewLine, string.Empty);
        }
    }
}
