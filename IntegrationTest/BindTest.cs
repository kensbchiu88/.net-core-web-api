using Microsoft.AspNetCore.Mvc.Testing;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PolarBearEapApi.Infra;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.PublicApi.Models;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Entities;

namespace IntegrationTest
{
    public  class BindTest
    {
        private HttpClient _httpClient;
        private const int TokenExpireHours = 4;
        private const string HWD = "36bd2cd3-3c94-4d53-a494-79eab5d34e9f";
        private const string INDICATOR = "ADD_RECORD";
        private const string LINE_CODE = "E08-1FT-01";
        private const string SECTION_CODE = "T04A-STATION81";
        private const int STATION_CODE = 72607;
        private const string OP_CATEGORY = "BIND";
        private const string SERVER_VERSION = "ServerVersion";

        public BindTest() 
        {
            var webApplicationFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(host =>
                {
                    host.ConfigureServices(services =>
                    {
                        var dbContextDescriptor = services.SingleOrDefault(
                     d => d.ServiceType ==
                         typeof(DbContextOptions<EapTokenDbContext>));

                        if (dbContextDescriptor != null)
                            services.Remove(dbContextDescriptor);


                        services.AddDbContext<EapTokenDbContext>(options =>
                        {
                            options.UseInMemoryDatabase("InMemoryEmployeeTest");
                        });
                    });
                });
            _httpClient = webApplicationFactory.CreateDefaultClient();
        }

        /** 
         * 測試沒有帶Hwd(token)
         * Given: Hwd為空的json request
         * Then: 1.Hwd為空, 
         *       2.OPResponseInfo.Result = "NG",
         *       3.Display不為空, 
         */
        [Fact]
        public async Task TestEmptyHwd() 
        {
            var requestModel = new ApiRequest() { };
            var endPoint = "/api/v1/EapApi";
            var requestBody = FakeEmptyHwdApiRequest();
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endPoint, content);

            var result = await response.Content.ReadAsStringAsync();
            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Empty(JsonUtil.GetCaseSensitiveParameter(result, "Hwd"));
            Assert.Equal("NG", JsonUtil.GetParameter(JsonUtil.GetParameter(result, "SerializeData"), "OPResponseInfo.Result"));
            Assert.Contains(ErrorCodeEnum.InvalidTokenFormat.ToString(), JsonUtil.GetCaseSensitiveParameter(result, "Display"));
        }

        /** 
         * 測試帶入的Hwd值沒有在Table中
         * Given: 正確的json request, 但Hwd值不存在DB中
         * Then: 1.Hwd與request中的Hwd相同, 
         *       2.OPResponseInfo.Result = "NG",
         *       3.Display=InvalidToken
        */
        [Fact]
        public async Task TestInvalidToken()
        {
            var requestModel = new ApiRequest() { };
            var endPoint = "/api/v1/EapApi";
            var requestBody = FakeInvalidTokenApiRequest();
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endPoint, content);

            var result = await response.Content.ReadAsStringAsync();
            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Equal(HWD, JsonUtil.GetCaseSensitiveParameter(result, "Hwd"));
            Assert.Equal("NG", JsonUtil.GetParameter(JsonUtil.GetParameter(result, "SerializeData"), "OPResponseInfo.Result"));
            Assert.Equal(ErrorCodeEnum.InvalidToken.ToString(), JsonUtil.GetCaseSensitiveParameter(result, "Display"));
        }
        /** 
         * 測試帶入的Hwd值有在Table中,但過期了
         * Given: 正確的json request, 但Hwd已經過期
         * Then: 1.Hwd與request中的Hwd相同, 
         *       2.OPResponseInfo.Result = "NG",
         *       3.Display=TokenExpired
        */
        
        //Deprecate. 在設定檔中已取消TokenExpired
        /*
        [Fact]
        public async Task TestTokenExpired()
        {
            //新增測試資料到DB中
            Guid guid = Guid.NewGuid();
            var options = new DbContextOptionsBuilder<EapTokenDbContext>().UseInMemoryDatabase(databaseName: "InMemoryEmployeeTest").Options;
            using (var context = new EapTokenDbContext(options))
            {
                context.EapTokenEntities.Add(FakeEapTokenEntity(guid));
                await context.SaveChangesAsync();
            }

            var requestModel = new ApiRequest() { };
            var endPoint = "/api/v1/EapApi";
            var requestBody = FakeApiResponse(guid.ToString());
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endPoint, content);

            var result = await response.Content.ReadAsStringAsync();
            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Equal(guid.ToString(), JsonUtil.GetCaseSensitiveParameter(result, "Hwd"));
            Assert.Equal("NG", JsonUtil.GetParameter(JsonUtil.GetParameter(result, "SerializeData"), "OPResponseInfo.Result"));
            Assert.Contains(ErrorCodeEnum.TokenExpired.ToString(), JsonUtil.GetCaseSensitiveParameter(result, "Display"));

            //移除DB中測試資料，避免影響其他測試
            using (var context = new EapTokenDbContext(options))
            {
                var entities = context.EapTokenEntities.ToList();
                var entityToDelete = context.EapTokenEntities.FirstOrDefault(e => e.Id == guid);

                if (entityToDelete != null)
                {
                    context.EapTokenEntities.Remove(entityToDelete);
                    await context.SaveChangesAsync();
                }
            }

        }
        */

        
        private static string FakeEmptyHwdApiRequest()
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

            JObject apiRquest = new JObject();
            apiRquest["Hwd"] = "";
            apiRquest["Indicator"] = INDICATOR;
            apiRquest["SerializeData"] = serializeData.ToString(Formatting.None);

            return apiRquest.ToString().Replace(Environment.NewLine, string.Empty);
        }

        private static string FakeInvalidTokenApiRequest()
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

            JObject apiRquest = new JObject();
            apiRquest["Hwd"] = HWD;
            apiRquest["Indicator"] = INDICATOR;
            apiRquest["SerializeData"] = serializeData.ToString(Formatting.None);

            return apiRquest.ToString().Replace(Environment.NewLine, string.Empty);
        }

        private static string FakeApiResponse(string hwd) 
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

            JObject apiRquest = new JObject();
            apiRquest["Hwd"] = hwd;
            apiRquest["Indicator"] = INDICATOR;
            apiRquest["SerializeData"] = serializeData.ToString(Formatting.None);

            return apiRquest.ToString().Replace(Environment.NewLine, string.Empty);
        }

        private static EapTokenEntity FakeEapTokenEntity(Guid guid) 
        {
            var tokenExpiredHours = TokenExpireHours + 1;
            EapTokenEntity entity = new EapTokenEntity();
            entity.Id = guid;
            entity.username = "username";
            entity.LoginTime = DateTime.Now.AddHours(0 - tokenExpiredHours);
            return entity;
        }
    }
}
