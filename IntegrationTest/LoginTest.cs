using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using PolarBearEapApi.Infra;
using Microsoft.Extensions.DependencyInjection;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.PublicApi.Models;
using System.Text;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Constants;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace IntegrationTest
{
    public class LoginTest
    {
        private HttpClient _httpClient;
        private const string INDICATOR = "QUERY_RECORD";
        private const string OP_CATEGORY = "LOGIN";
        private const string CORRECT_USERNAME = "V0228172";
        private const string CORRECT_PASSWORD = "V0228172";
        private const string WRONG_USERNAME = "bbbbb";
        private const string WRONG_PASSWORD = "aaaaa";
        private const string IN_MEMORY_DB_NAME = "LoginTest";

        public LoginTest()
        {
            var webApplicationFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(host =>
                {
                    host.ConfigureServices(services =>
                    {
                        var dbContextDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<EapTokenDbContext>));

                        if (dbContextDescriptor != null)
                            services.Remove(dbContextDescriptor);


                        services.AddDbContext<EapTokenDbContext>(options =>
                        {
                            options.UseInMemoryDatabase(IN_MEMORY_DB_NAME);
                        });
                    });
                });
            _httpClient = webApplicationFactory.CreateDefaultClient();
        }

        /** 
         * 測試登入成功 Run fail可能因為帳密已被修改，若如此則把此測試mark掉
         * Given: 正確的Login Request
         * Then: 1.SerializeData.OPResponseInfo.Hwd有值, 
         *       2.Display:null, 
         *       3.資料庫中有一筆資料
         *       4.資料庫中資料的Id與Response中的Hwd相同，username也與request中的相同.
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
            Assert.NotEmpty(JsonUtil.GetParameter(JsonUtil.GetParameter(result, "SerializeData"), "OPResponseInfo.Hwd"));
            Assert.Empty(JsonUtil.GetParameter(result, "Display"));

            var token = JsonUtil.GetCaseSensitiveParameter(JsonUtil.GetCaseSensitiveParameter(result, "SerializeData"), "OPResponseInfo.Hwd");
            var options = new DbContextOptionsBuilder<EapTokenDbContext>().UseInMemoryDatabase(databaseName: IN_MEMORY_DB_NAME).Options;
            using (var context = new EapTokenDbContext(options))
            {
                var entities = context.EapTokenEntities.ToList();

                // 驗證數據
                Assert.Single(entities);
                Assert.Equal(token, entities[0].Id.ToString());
                Assert.Equal(CORRECT_USERNAME, entities[0].username);

                //移除DB中測試資料，避免影響其他測試
                if (entities != null)
                {
                    context.EapTokenEntities.Remove(entities[0]);
                    await context.SaveChangesAsync();
                }
            }
        }

        /** 
         * 測試登入成功 Run fail可能因為帳密已被修改，若如此則把此測試mark掉
         * Given: 錯誤帳密的的Login Request
         * Then: 1.SerializeData.OPResponseInfo.Hwd為空, 
         *       2.Display:LOGINFAIL
         */
        [Fact]
        public async Task TestLoginFail()
        {
            var requestModel = new ApiRequest() { };
            var endPoint = "/api/v1/EapApi";
            var requestBody = FakeWrongCredentialApiRequest();
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync(endPoint, content);

            var result = await response.Content.ReadAsStringAsync();
            Assert.True(!string.IsNullOrEmpty(result));
            Assert.Empty(JsonUtil.GetParameter(JsonUtil.GetParameter(result, "SerializeData"), "OPResponseInfo.Hwd"));
            Assert.Equal(ErrorCodeEnum.LoginFail.ToString(), JsonUtil.GetCaseSensitiveParameter(result, "Display"));
        }

        private static string FakeApiRequest()
        {
            JObject opRequestInfo = new JObject();
            opRequestInfo["user"] = CORRECT_USERNAME;
            opRequestInfo["pwd"] = CORRECT_PASSWORD;

            JObject serializeData = new JObject();
            serializeData["LineCode"] = "";
            serializeData["SectionCode"] = "";
            serializeData["StationCode"] = null;
            serializeData["OPCategory"] = OP_CATEGORY;
            serializeData["OPRequestInfo"] = opRequestInfo;
            serializeData["OPResponseInfo"] = new JObject();

            JObject apiRquest = new JObject();
            apiRquest["Hwd"] = "";
            apiRquest["Indicator"] = INDICATOR;
            apiRquest["SerializeData"] = serializeData.ToString(Formatting.None);

            return apiRquest.ToString().Replace(Environment.NewLine, string.Empty);
        }

        private static string FakeWrongCredentialApiRequest()
        {
            JObject opRequestInfo = new JObject();
            opRequestInfo["user"] = WRONG_USERNAME;
            opRequestInfo["pwd"] = WRONG_PASSWORD;

            JObject serializeData = new JObject();
            serializeData["LineCode"] = "";
            serializeData["SectionCode"] = "";
            serializeData["StationCode"] = null;
            serializeData["OPCategory"] = OP_CATEGORY;
            serializeData["OPRequestInfo"] = opRequestInfo;
            serializeData["OPResponseInfo"] = new JObject();

            JObject apiRquest = new JObject();
            apiRquest["Hwd"] = "";
            apiRquest["Indicator"] = INDICATOR;
            apiRquest["SerializeData"] = serializeData.ToString(Formatting.None);

            return apiRquest.ToString().Replace(Environment.NewLine, string.Empty);
        }
    }
}
