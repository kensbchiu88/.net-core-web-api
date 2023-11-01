using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.PublicApi.Filters
{
    public class SimpleFormatExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            var responseModel = new SimpleResponse<object>
            {
                Result = "NG",
                Message = context.Exception.Message
            };
            var body = JsonConvert.SerializeObject(responseModel);
            context.HttpContext.Response.WriteAsync(body);
        }

        public async Task OnExceptionAsync(ExceptionContext context)
        {
            var response = context.HttpContext.Response;
            response.ContentType = "application/json";
            var responseModel = new SimpleResponse<object>
            {
                Result = "NG",
                Message = context.Exception.Message
            };
            var body = JsonConvert.SerializeObject(responseModel);
            await context.HttpContext.Response.WriteAsync(body);
        }
    }
}
