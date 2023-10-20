using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Entities;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.Infra;
using PolarBearEapApi.PublicApi.Models;
using System.Globalization;
using System.Text;

namespace IntegrationTest
{
    public class GetInputDataTest
    {
        private HttpClient _httpClient;

        private const string HWD = "36bd2cd3-3c94-4d53-a494-79eab5d34e9f";
        private const string INDICATOR = "QUERY_RECORD";
        private const string OP_CATEGORY = "GET_INPUT_DATA";
        private const string LINE_CODE = "TEST_LINE";
        private const string SECTION_CODE = "TEST_SECTION";
        private const int STATION_CODE = 111;
        private const string SN = "TEST_SN";
        private const string OP_REQUEST_INFO = "{\"A1\": \"A\", \"A2\": \"AA\"}";
        private static DateTime UPLOAD_TIME = DateTime.Now;

        private const string NO_DATA_FOUND_SN = "NO_DATA_FOUND_SN";

        public GetInputDataTest() 
        {
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
         * Given: 新增測試資料到DB中，及正確的Json格式
         * Then: 1.Hwd與input時相同, 
         *       2.OPResponseInfo.Data與測試資料的OP_REQUEST_INFO相同,
         *       3.OPResponseInfo.InputDate與測試資料的UPLOAD_TIME相同,
         *       4.Display:null
         */
        [Fact]
        public async Task TestSuccess()
        {
            //新增測試資料到DB中
            var options = new DbContextOptionsBuilder<UploadInfoDbContext>().UseInMemoryDatabase(databaseName: "InMemoryEmployeeTest").Options;
            using (var context = new UploadInfoDbContext(options))
            {
                context.UploadInfoEnties.Add(FakeUploadInfoEntity());
                await context.SaveChangesAsync();
            }

            var requestModel = new ApiRequest() { };
            var endPoint = "/api/v1/EapApi";
            var requestBody = FakeApiRequest();
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endPoint, content);

            var result = await response.Content.ReadAsStringAsync();
            var inputDateString = JsonUtil.GetParameter(JsonUtil.GetParameter(result, "SerializeData"), "OPResponseInfo.InputDate");
            var inputDate = DateTime.ParseExact(inputDateString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            var uploadtime = new DateTime(UPLOAD_TIME.Year, UPLOAD_TIME.Month, UPLOAD_TIME.Day, UPLOAD_TIME.Hour, UPLOAD_TIME.Minute, UPLOAD_TIME.Second);

            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Equal(HWD, JsonUtil.GetCaseSensitiveParameter(result, "Hwd"));
            Assert.Equal(OP_REQUEST_INFO, JsonUtil.GetParameter(JsonUtil.GetParameter(result, "SerializeData"), "OPResponseInfo.Data"));
            Assert.True(DateTime.Compare(uploadtime, inputDate) == 0);
            Assert.Empty(JsonUtil.GetParameter(result, "Display"));
        }

        /** 
         * 測試找不到資料
         * Given: 給一個不存在的SN及正確的Json格式
         * Then: 1.Hwd與input時相同, 
         *       2.OPResponseInfo.Data等於"{}",
         *       3.無OPResponseInfo.InputDate,
         *       4.Display:NoDataFound
         */
        [Fact]
        public async Task TestNoDataFound()
        {
            var requestModel = new ApiRequest() { };
            var endPoint = "/api/v1/EapApi";
            var requestBody = FakeNoDataFoundApiRequest();
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endPoint, content);

            var result = await response.Content.ReadAsStringAsync();

            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Equal(HWD, JsonUtil.GetCaseSensitiveParameter(result, "Hwd"));
            Assert.Equal("{}", JsonUtil.GetParameter(JsonUtil.GetParameter(result, "SerializeData"), "OPResponseInfo.Data"));
            Assert.Empty(JsonUtil.GetParameter(JsonUtil.GetParameter(result, "SerializeData"), "OPResponseInfo.InputDate"));
            Assert.Equal(ErrorCodeEnum.NoDataFound.ToString(), JsonUtil.GetCaseSensitiveParameter(result, "Display"));
        }
         private static UploadInfoEntity FakeUploadInfoEntity()
        {
            UploadInfoEntity entity = new UploadInfoEntity();
            entity.LineCode = LINE_CODE;
            entity.SectionCode = SECTION_CODE;
            entity.StationCode = STATION_CODE;
            entity.Sn = SN;
            entity.OpRequestInfo = OP_REQUEST_INFO;
            entity.UploadTime = UPLOAD_TIME;

            return entity;
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

        private static string FakeNoDataFoundApiRequest()
        {
            JObject opRequestInfo = new JObject();
            opRequestInfo["SN"] = NO_DATA_FOUND_SN;

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
