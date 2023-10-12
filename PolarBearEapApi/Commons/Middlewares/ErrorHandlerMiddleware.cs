using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using PolarBearEapApi.Commons.Exceptions;
using PolarBearEapApi.Models;
using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;

namespace PolarBearEapApi.Commons.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlerMiddleware> _logger;
        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            ApiRequest? requestModel = new ApiRequest();
            string? requestHwd = string.Empty;
            string? requestIndicator = string.Empty;
            string? requestSerializeData = string.Empty;
            try
            {
                var request = context.Request;
                if (request.Method == HttpMethods.Post && request.ContentLength > 0)
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
                            requestHwd = requestModel.Hwd;
                            requestIndicator = requestModel.Indicator;
                            requestSerializeData = ResponseGenerator.Fail(requestModel.SerializeData);
                        }
                    }
                }

                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                var responseModel = new ApiResponse();
                responseModel.Hwd = requestHwd;
                responseModel.Indicator = requestIndicator;
                responseModel.SerializeData = requestSerializeData;

                switch (error)
                {
                    case EapJsonParseException e:
                        responseModel.Display = ErrorCodeEnum.ParseJsonError.ToString();
                        break;
                    case TokenExpireException e1:
                        responseModel.Display = ErrorCodeEnum.TokenExpired.ToString();
                        break;
                    case InvalidTokenException e2:
                        responseModel.Display = ErrorCodeEnum.InvalidToken.ToString();
                        break;
                    default:
                        // unhandled error
                        responseModel.Display = error.Message;
                        break;
                }

                _logger.LogError(LogMessageGenerator.GetErrorMessage(requestSerializeData, error.StackTrace ?? ""));
                _logger.LogError(LogMessageGenerator.GetErrorMessage(requestSerializeData, error.Message));

                var result = JsonConvert.SerializeObject(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}
