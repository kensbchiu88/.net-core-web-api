﻿using Newtonsoft.Json;
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

        public async Task InvokeAsync(HttpContext context, ITokenRepository tokenService)
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

            await _next(context);
        }

    }
}
