﻿using Newtonsoft.Json;
using PolarBearEapApi.ApplicationCore.Constants;
using PolarBearEapApi.ApplicationCore.Exceptions;
using PolarBearEapApi.ApplicationCore.Extensions;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.PublicApi.Middlewares
{
    /** 處理Exception，將Exception轉換成Api Response的格式，避免Exception直接回傳到Client */
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
            var request = context.Request;

            //log Response for Debug
            #if DEBUG
                var (originalBody, responseBodyStream) = AssignMemoryStreamToResponse(context);
            #endif
            try
            {
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
                if (!request.Path.ToString().Contains("soap") && request.Method == HttpMethods.Post && request.ContentLength > 0)
                {
                    var response = context.Response;
                    response.ContentType = "application/json";
                    string result = string.Empty;
                    //SerializeData不為空，Response是ApiResponse格式, 若是JsonException, 也以ApiResponse格式呈現
                    if (!string.IsNullOrEmpty(requestSerializeData) || error is JsonException)
                    {
                        var responseModel = new ApiResponse();

                        responseModel.Hwd = requestHwd;
                        responseModel.Indicator = requestIndicator;
                        switch (error)
                        {
                            case JsonException:
                                responseModel.SerializeData = ResponseSerializeDataGenerator.GenerateEmptySerializeData();
                                responseModel.Display = ErrorCodeEnum.ParseJsonError.ToString() + ": " + error.Message;
                                break;
                            case EapException:
                                try
                                {
                                    responseModel.SerializeData = ResponseSerializeDataGenerator.Fail(requestSerializeData);
                                }
                                catch
                                {
                                    responseModel.SerializeData = requestSerializeData;
                                }
                                responseModel.Display = error.Message;
                                break;
                            default:
                                responseModel.SerializeData = requestSerializeData;
                                responseModel.Display = error.Message;
                                break;
                        }
                        _logger.LogError(LogMessageGenerator.GetErrorMessage(requestSerializeData, error.ToString()));

                        result = JsonConvert.SerializeObject(responseModel);
                    }
                    else //反之Response是SimpleResponse格式
                    {
                        var responseModel = new SimpleResponse<object>
                        {
                            Result = "NG",
                            Message = error.Message
                        };

                        _logger.LogError(error.ToString());
                        result = JsonConvert.SerializeObject(responseModel);
                    }

                    await response.WriteAsync(result);
                }
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
            _logger.LogInformation($"ErrorHandlerMiddlewares Response: {responseBody}");

            // 恢复原始响应流
            context.Response.Body = originalBody;
            responseBodyStream.Dispose();
        }
    }
}
