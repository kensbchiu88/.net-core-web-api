﻿using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.PublicApi.Middlewares
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
                            requestSerializeData = requestModel.SerializeData;
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
                responseModel.SerializeData = ResponseSerializeDataGenerator.Fail(requestSerializeData);

                responseModel.Display = error.Message;


                /*
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
                    case InvalidTokenFormatException e3:
                        responseModel.Display = ErrorCodeEnum.InvalidTokenFormat.ToString();
                        break;
                    default:
                        // unhandled error
                        responseModel.Display = error.Message;
                        break;
                }
                */

                _logger.LogError(LogMessageGenerator.GetErrorMessage(requestSerializeData, error.StackTrace ?? ""));
                _logger.LogError(LogMessageGenerator.GetErrorMessage(requestSerializeData, error.Message));

                var result = JsonConvert.SerializeObject(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}
