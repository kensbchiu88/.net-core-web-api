using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.ApplicationCore.Interfaces;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.PublicApi.Middlewares
{
    /** 檢查Token */
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<TokenMiddleware> _logger;

        public TokenMiddleware(RequestDelegate next, ILogger<TokenMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context, ITokenRepository tokenService)
        {
            List<string> validateList = new List<string>(new string[] { "UNIT_PROCESS_CHECK", "BIND", "GET_SN_BY_SN_FIXTURE" });

            ApiRequest? requestModel = new ApiRequest();

            var request = context.Request;
            if (!request.Path.ToString().Contains("soap") && request.Method == HttpMethods.Post && request.ContentLength > 0)
            {
                request.EnableBuffering();

                using (var requestReader = new StreamReader(request.Body, leaveOpen: true))
                {
                    var body = await requestReader.ReadToEndAsync();
                    // 重要, 才能允許回捲 (request.Body.Seek(0, SeekOrigin.Begin))
                    request.Body.Seek(0, SeekOrigin.Begin);

                    requestModel = JsonConvert.DeserializeObject<ApiRequest>(body);

                    if (requestModel != null)
                    {
                        var requestOpCategory = JsonUtil.GetParameter(requestModel.SerializeData, "OPCategory");
                        if (requestOpCategory != null)
                        {
                            if (validateList.Contains(requestOpCategory.ToUpper()))
                            {
                                await tokenService.Validate(requestModel.Hwd);
                            }
                        }
                    }
                }
            }

            //log Response for Debug
            #if DEBUG
                var (originalBody, responseBodyStream) = AssignMemoryStreamToResponse(context);
            #endif

            try
            {
                await _next(context);
            }
            finally
            {
                //log Response for Debug
                #if DEBUG
                    await LogResponseAndRecoverResponse(context, originalBody, responseBodyStream);
                #endif
            }          
        }

        private (Stream originalBody, MemoryStream responseBodyStream) AssignMemoryStreamToResponse(HttpContext context)
        {
            var originalBody = context.Response.Body;
            var responseBodyStream = new MemoryStream();
            context.Response.Body = responseBodyStream;

            return (originalBody, responseBodyStream);
        }

        private async Task LogResponseAndRecoverResponse(HttpContext context, Stream originalBody, MemoryStream responseBodyStream) 
        {
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();

            // 将响应内容写回原始响应流
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBody);

            // 记录响应内容，这里可以根据需要进行日志记录或其他处理
            _logger.LogInformation($"TokenMiddlewares Response: {responseBody}");

            // 恢复原始响应流
            context.Response.Body = originalBody;
            responseBodyStream.Dispose();
        }
    }
}
