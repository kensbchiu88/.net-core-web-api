using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using PolarBearEapApi.PublicApi.Middlewares;
using Serilog.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolarBearEapApiUnitTests.PublicApi.Middlewares
{
    public class LoggerMiddlewareTest
    {
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
        public async Task Test()
        {
            var mockLogger = new Mock<ILogger<LoggerMiddleware>>();
            var requestDelegate = new RequestDelegate((HttpContext httpContext) =>
            {
                return Task.CompletedTask;
            });

            var middleware = new LoggerMiddleware(requestDelegate, mockLogger.Object);

            // Act
            var exception = await Record.ExceptionAsync(() => middleware.Invoke(new DefaultHttpContext()));

            //Assert 
            mockLogger.Verify(l =>
                l.Log(LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Api Request")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ), Times.Once
            );

            mockLogger.Verify(l =>
                l.Log(LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, _) => v.ToString().Contains("Api Response")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception, string>>()
                ), Times.Once
            );
        }
        
    }
}
