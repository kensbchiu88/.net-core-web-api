using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Middlewares;
using System.Net;
using System.Text;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Constants;

namespace PolarBearEapApiUnitTests.PublicApi.Middlewares
{
    public class TokenMiddlewareTest
    {
        private const string BIND_REQUEST_BODY = "{\"Hwd\": \"B36C976F-23BB-49E3-B567-4D2449EFA7F6\",\"Indicator\": \"ADD_RECORD\",\"SerializeData\": \"{\\\"LineCode\\\":\\\"E08-1FT-01\\\",\\\"SectionCode\\\":\\\"O-01\\\",\\\"StationCode\\\":12346,\\\"OPCategory\\\":\\\"BIND\\\",\\\"OPRequestInfo\\\":{\\\"ServerVersion\\\":\\\"v1.01\\\"},\\\"OPResponseInfo\\\":{}}\"}";
        private const string UNIT_PROCESS_CHEDK_REQUEST_BODY = "{\"Hwd\":\"36bd2cd3-3c94-4d53-a494-79eab5d34e9f\",\"Indicator\": \"QUERY_RECORD\",\"SerializeData\": \"{\\\"LineCode\\\":\\\"E08-1FT-01\\\",\\\"SectionCode\\\":\\\"T04A-STATION432\\\",\\\"StationCode\\\":72617,\\\"OPCategory\\\":\\\"UNIT_PROCESS_CHECK\\\",\\\"OPRequestInfo\\\":{\\\"SN\\\":\\\"J21GYM001M100000US\\\"},\\\"OPResponseInfo\\\":{}}\"}";
        private const string GET_SN_BY_SN_FIXTURE_REQUEST_BODY = "{\"Hwd\": \"36bd2cd3-3c94-4d53-a494-79eab5d34e9f\",\"Indicator\": \"QUERY_RECORD\",\"SerializeData\": \"{\\\"LineCode\\\":\\\"E08-1FT-01\\\",\\\"SectionCode\\\":\\\"T04A-STATION81\\\",\\\"StationCode\\\":72607,\\\"OPCategory\\\":\\\"GET_SN_BY_SN_FIXTURE\\\",\\\"OPRequestInfo\\\":{\\\"REF_VALUE\\\":\\\"UC030-0001-0001\\\",\\\"REF_TYPE\\\":\\\"SN_FIXTURE\\\"},\\\"OPResponseInfo\\\":{}}\"}";
        private const string GET_INPUT_DATA_REQUEST_BODY = "{\"Hwd\": \"36bd2cd3-3c94-4d53-a494-79eab5d34e9f\",\"Indicator\": \"QUERY_RECORD\",\"SerializeData\": \"{\\\"LineCode\\\":\\\"E08-1FT-01\\\",\\\"SectionCode\\\":\\\"T04A-STATION81\\\",\\\"StationCode\\\":72607,\\\"OPCategory\\\":\\\"GET_INPUT_DATA\\\",\\\"OPRequestInfo\\\":{\\\"SN\\\":\\\"J21GYM0028100000US\\\"},\\\"OPResponseInfo\\\":{}}\"}";

        /** 
         * BIND 會驗證TOKEN
         * Given: BIND request, 且ITokenService.Validate throw InvalidTokenException
         * Then: throw InvalidTokenException
         */
        [Fact]
        public async Task TestBindWillValidateToken() 
        {
            // Arrange
            var mockLogger = new Mock<ILogger<TokenMiddleware>>();
            var context = FakeHttpContext(BIND_REQUEST_BODY);
            var mockTokenService = new Mock<ITokenRepository>();
            mockTokenService.Setup(service => service.Validate(It.IsAny<string>())).Throws(new EapException(ErrorCodeEnum.InvalidToken));

            var requestDelegate = new RequestDelegate((HttpContext httpContext) =>
            {                               
                return Task.CompletedTask;
            });

            var middleware = new TokenMiddleware(requestDelegate, mockLogger.Object);

            // Act
            var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(context, mockTokenService.Object));

            //assert
            Assert.NotNull(exception);
            Assert.IsType<EapException>(exception);
            Assert.Contains(ErrorCodeEnum.InvalidToken.ToString(), exception.Message);
        }

        /** 
         * UNIT_PROCESS_CHEDK 會驗證TOKEN
         * Given: UNIT_PROCESS_CHEDK request, 且ITokenService.Validate throw InvalidTokenException
         * Then: throw InvalidTokenException
         */
        [Fact]
        public async Task TestUnitProcessCheckWillValidateToken()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<TokenMiddleware>>();
            var context = FakeHttpContext(UNIT_PROCESS_CHEDK_REQUEST_BODY);
            var mockTokenService = new Mock<ITokenRepository>();
            mockTokenService.Setup(service => service.Validate(It.IsAny<string>())).Throws(new EapException(ErrorCodeEnum.InvalidToken));

            var requestDelegate = new RequestDelegate((HttpContext httpContext) =>
            {
                return Task.CompletedTask;
            });

            var middleware = new TokenMiddleware(requestDelegate, mockLogger.Object);

            // Act
            var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(context, mockTokenService.Object));

            //assert
            Assert.NotNull(exception);
            Assert.IsType<EapException>(exception);
            Assert.Contains(ErrorCodeEnum.InvalidToken.ToString(), exception.Message);
        }


        /** 
         * GET_SN_BY_SN_FIXTURE 會驗證TOKEN
         * Given: GET_SN_BY_SN_FIXTURE request, 且ITokenService.Validate throw InvalidTokenException
         * Then: throw InvalidTokenException
         */
        [Fact]
        public async Task TestGetSnByFixtureWillValidateToken()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<TokenMiddleware>>();
            var context = FakeHttpContext(GET_SN_BY_SN_FIXTURE_REQUEST_BODY);
            var mockTokenService = new Mock<ITokenRepository>();
            mockTokenService.Setup(service => service.Validate(It.IsAny<string>())).Throws(new EapException(ErrorCodeEnum.InvalidToken));

            var requestDelegate = new RequestDelegate((HttpContext httpContext) =>
            {
                return Task.CompletedTask;
            });

            var middleware = new TokenMiddleware(requestDelegate, mockLogger.Object);

            // Act
            var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(context, mockTokenService.Object));

            //assert
            Assert.NotNull(exception);
            Assert.IsType<EapException>(exception);
            Assert.Contains(ErrorCodeEnum.InvalidToken.ToString(), exception.Message);
        }

        /** 
         * GET_INPUT_DATA 不會驗證TOKEN
         * Given: GET_INPUT_DATA request, 且ITokenService.Validate throw InvalidTokenException
         * Then: 不會丟出InvalidTokenException
         */
        [Fact]
        public async Task TestGetInputDataNoValidateToken()
        {
            // Arrange
            var mockLogger = new Mock<ILogger<TokenMiddleware>>();
            var context = FakeHttpContext(GET_INPUT_DATA_REQUEST_BODY);
            var mockTokenService = new Mock<ITokenRepository>();
            mockTokenService.Setup(service => service.Validate(It.IsAny<string>())).Throws(new EapException(ErrorCodeEnum.InvalidToken));

            var requestDelegate = new RequestDelegate((HttpContext httpContext) =>
            {
                return Task.CompletedTask;
            });

            var middleware = new TokenMiddleware(requestDelegate, mockLogger.Object);

            //assert
            var exception = await Record.ExceptionAsync(() => middleware.InvokeAsync(context, mockTokenService.Object));
            Assert.Null(exception);
            Assert.Equal(HttpStatusCode.OK, (HttpStatusCode)context.Response.StatusCode);
        }

        private static HttpContext FakeHttpContext(string requestBody)
        {
            var context = new DefaultHttpContext();
            context.Request.Method = HttpMethods.Post;
            context.Request.ContentType = "application/json";
            var requestStream = new MemoryStream(Encoding.UTF8.GetBytes(requestBody));
            context.Request.Body = requestStream;
            context.Request.ContentLength = requestStream.Length;

            return context;
        }
    }
}
