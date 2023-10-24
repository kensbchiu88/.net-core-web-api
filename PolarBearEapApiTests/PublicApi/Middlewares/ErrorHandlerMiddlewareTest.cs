using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Middlewares;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PolarBearEapApiUnitTests.PublicApi.Middlewares
{
    public class ErrorHandlerMiddlewareTest
    {
        private const string HWD = "36bd2cd3-3c94-4d53-a494-79eab5d34e9f";
        private const string INDICATOR = "ADD_RECORD";
        private const string LINE_CODE = "E08-1FT-01";
        private const string SECTION_CODE = "T04A-STATION81";
        private const int STATION_CODE = 72607;
        private const string OP_CATEGORY = "BIND";
        private const string SERVER_VERSION = "ServerVersion";

        /** 
        * 測試當程式丟出Exception
        * Given: valid json request, 且RequestDelegate丟出exception
        * Then: response中的 1. 不丟出Exception
        *                    2. Hwd與input相同
        *                    3. Indecator與input相同
        *                    4. result = NG (因為Input是BIND，所以Fail時的Result是NG)
        *                    5. display is not null                        
        */
        [Fact]
        public async Task TestGeneralException()
        {
            // Arrange
            var readableBodyStream = new MemoryStream();
            var mockLogger = new Mock<ILogger<ErrorHandlerMiddleware>>();
            var requestDelegate = new RequestDelegate((HttpContext httpContext) =>
            {
                return Task.FromException(new EapException(ErrorCodeEnum.ParseJsonError));
            });                   
            var context = FakeHttpContext(FakeApiRequest(), readableBodyStream);               
            var middleware = new ErrorHandlerMiddleware(requestDelegate, mockLogger.Object);

            // Act
            var exception = await Record.ExceptionAsync(() => middleware.Invoke(context));

            //assert
            readableBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(readableBodyStream).ReadToEndAsync();

            Assert.Null(exception);
            Assert.Equal(HWD, JsonUtil.GetCaseSensitiveParameter(responseBody, "Hwd"));
            Assert.Equal(INDICATOR, JsonUtil.GetParameter(responseBody, "Indicator"));
            Assert.Equal("NG", JsonUtil.GetParameter(JsonUtil.GetParameter(responseBody, "SerializeData")!, "OPResponseInfo.Result"));
            Assert.NotEmpty(JsonUtil.GetCaseSensitiveParameter(responseBody, "Display")!);
            
            readableBodyStream.Close();
        }

        private static HttpContext FakeHttpContext(string requestBody, MemoryStream readableBodyStream)
        {
            var context = new DefaultHttpContext();
            context.Request.Method = HttpMethods.Post;
            context.Request.ContentType = "application/json";
            var requestStream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
            context.Request.Body = requestStream;
            context.Request.ContentLength = requestStream.Length;
            context.Response.Body = readableBodyStream;

            return context;
        }

        private static string FakeApiRequest()
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
    }
}
