using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using PolarBearEapApi.PublicApi.Models;

namespace PolarBearEapApi.PublicApi.Filters
{
    /** 將API的Response轉換成 {"Result" : "", "Data" : "", "Message" : ""}格式。供非GTK定義API使用。*/
    public class SimpleResponseRewriteActionFilter : IAsyncActionFilter
    {        
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var executedContext = await next(); // 执行操作方法

            if (executedContext.Result is ObjectResult objectResult)
            {
                if (objectResult.Value is SimpleResponse<object>)
                {
                    // 如果已经是 SimpleResponse<T>，则不进行包装
                    return;
                }

                var data = objectResult.Value;

                // 使用泛型方法包装响应
                var simpleResponse = WrapResponse(data);

                // 创建新的 ObjectResult，将 SimpleResponse<T> 作为值
                var newObjectResult = new ObjectResult(simpleResponse)
                {
                    StatusCode = objectResult.StatusCode,
                    DeclaredType = simpleResponse.GetType() // 设置 SimpleResponse<T> 的类型
                };

                // 替换原始的 ActionResult
                executedContext.Result = newObjectResult;
            }
            else if (executedContext.Result is EmptyResult)
            {
                // 使用泛型方法包装响应
                var simpleResponse = WrapResponse<object>(null);

                // 创建新的 ObjectResult，将 SimpleResponse<T> 作为值
                var newObjectResult = new ObjectResult(simpleResponse)
                {
                    DeclaredType = simpleResponse.GetType() // 设置 SimpleResponse<T> 的类型
                };

                // 替换原始的 ActionResult
                executedContext.Result = newObjectResult;
            }
        }

        private SimpleResponse<T> WrapResponse<T>(T data)
        {
            // 提取常用的 Result 和 Message 值
            var result = "OK";

            return new SimpleResponse<T>
            {
                Result = result,
                Data = data,
                Message = null
            };
        }
    }

}
