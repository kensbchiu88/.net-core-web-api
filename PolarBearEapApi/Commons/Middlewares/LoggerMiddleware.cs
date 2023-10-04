using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PolarBearEapApi.Models;
using System.Diagnostics;
using System.Text;

namespace PolarBearEapApi.Commons.Middlewares
{
    public class LoggerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<LoggerMiddleware> _logger;
        public LoggerMiddleware(RequestDelegate next, ILogger<LoggerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var request = context.Request;

            request.EnableBuffering();

            using (var requestReader = new StreamReader(request.Body, leaveOpen: true))
            {
                var body = await requestReader.ReadToEndAsync();
                // 重要, 才能允許回捲 (request.Body.Seek(0, SeekOrigin.Begin))
                request.Body.Seek(0, SeekOrigin.Begin);

                _logger.LogInformation("Api Request :" + body);
            }


            // 捕获响应内容之前，需要先克隆原始响应流
            var originalBody = context.Response.Body;
            var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;


            try 
            {
                await _next(context);

                responseBodyStream.Seek(0, SeekOrigin.Begin);
                var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();

                // 记录响应内容，这里可以根据需要进行日志记录或其他处理
                _logger.LogInformation($"Api Response: {responseBody}");

                // 将响应内容写回原始响应流
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                await responseBodyStream.CopyToAsync(originalBody);
            }
            finally
            {
                // 恢复原始响应流
                context.Response.Body = originalBody;
            }
        }
    }
}
