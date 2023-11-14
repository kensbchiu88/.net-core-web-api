using Serilog.Context;

namespace PolarBearEapApi.PublicApi.Middlewares
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
                var requestBody = await requestReader.ReadToEndAsync();
                // 重要, 才能允許回捲 (request.Body.Seek(0, SeekOrigin.Begin))
                request.Body.Seek(0, SeekOrigin.Begin);

                _logger.LogInformation($"Api Request: {requestBody}");
            }


            // 捕获响应内容之前，需要先克隆原始响应流
            var originalBody = context.Response.Body;
            var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;
            string responseBody = string.Empty;


            try
            {
                await _next(context);

                responseBodyStream.Seek(0, SeekOrigin.Begin);
                responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();

                // 将响应内容写回原始响应流
                responseBodyStream.Seek(0, SeekOrigin.Begin);
                await responseBodyStream.CopyToAsync(originalBody);
            }
            catch(Exception e)
            {
                _logger.LogError(e.ToString());
            }
            finally
            {
                // 记录响应内容，这里可以根据需要进行日志记录或其他处理
                _logger.LogInformation($"Api Response: {responseBody}");

                // 恢复原始响应流
                context.Response.Body = originalBody;
                responseBodyStream.Dispose();
            }
        }
    }
}
