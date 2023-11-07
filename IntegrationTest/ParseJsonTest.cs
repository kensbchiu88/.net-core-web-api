using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Logging;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Extensions;
using System.Text;
using Moq;
using PolarBearEapApi.PublicApi.Middlewares;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTest
{
    public class ParseJsonTest
    {
        /** 
         * 測試Request轉換成ApiRequest發生錯誤
         * Given: 錯誤的Json String
         * Then: 1. Response中的Display不為NULL且包含ParseJsonError字串
         *       2. 呼叫SeriLog兩次  
         */
        [Fact]
        public async Task TestTranformApiRequestFail()
        {
            var logger = new Mock<ILogger<LoggerMiddleware>>();

            var webApplicationFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(host =>
                {
                    host.ConfigureServices(services =>
                    {
                        services.AddSingleton(logger.Object);
                    });
                });

            var client = webApplicationFactory.CreateDefaultClient();

            var endPoint = "/api/v1/EapApi";
            var requestBody = "{\"Hwd\": \"\",\"Indicator\": ?\"QUERY_RECORD\",\"SerializeData\": \"{\\\"LineCode\\\":\\\"\\\",\\\"SectionCode\\\":\\\"\\\",\\\"StationCode\\\":,\\\"OPCategory\\\":\\\"LOGIN\\\",\\\"OPRequestInfo\\\":{\\\"user\\\":\\\"my_username_example\\\",\\\"pwd\\\":\\\"my_pwd_example\\\"},\\\"OPResponseInfo\\\":{}}\"}";
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endPoint, content);

            var result = await response.Content.ReadAsStringAsync();
            Assert.True(!string.IsNullOrEmpty(result));
            Assert.NotEmpty(JsonUtil.GetParameter(result, "Display")!);
            Assert.Contains(ErrorCodeEnum.ParseJsonError.ToString().ToUpper(), JsonUtil.GetParameter(result, "Display"));
            logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),Times.Exactly(2));
        }

        /** 
         * 測試SerializeData中的Json格是錯誤，導致JObject.Parse時出現問題
         * Given: 錯誤的Json String
         * Then: 1. Response中的Display不為NULL且包含ParseJsonError字串
         *       2. 呼叫SeriLog兩次  
         */
        [Fact]
        public async Task TestJObjectParseFail()
        {
            var logger = new Mock<ILogger<LoggerMiddleware>>();

            var webApplicationFactory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(host =>
                {
                    host.ConfigureServices(services =>
                    {
                        services.AddSingleton(logger.Object);
                    });
                });

            var client = webApplicationFactory.CreateDefaultClient();

            var endPoint = "/api/v1/EapApi";
            var requestBody = "{\"Hwd\": \"\",\"Indicator\": \"QUERY_RECORD\",\"SerializeData\": \"{\\\"LineCode\\\":\\\"\\\"\\\"SectionCode\\\":\\\"\\\",\\\"StationCode\\\":,\\\"OPCategory\\\":\\\"LOGIN\\\",\\\"OPRequestInfo\\\":{\\\"user\\\":\\\"my_username_example\\\",\\\"pwd\\\":\\\"my_pwd_example\\\"},\\\"OPResponseInfo\\\":{}}\"}";
            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(endPoint, content);

            var result = await response.Content.ReadAsStringAsync();
            Assert.True(!string.IsNullOrEmpty(result));
            Assert.NotEmpty(JsonUtil.GetParameter(result, "Display")!);
            Assert.Contains(ErrorCodeEnum.ParseJsonError.ToString().ToUpper(), JsonUtil.GetParameter(result, "Display"));
            logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.IsAny<It.IsAnyType>(), null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()), Times.Exactly(2));
        }
    }
}
