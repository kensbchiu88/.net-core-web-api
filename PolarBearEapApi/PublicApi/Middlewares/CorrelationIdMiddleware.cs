using Serilog.Context;

namespace PolarBearEapApi.PublicApi.Middlewares
{
    /** 產生Guid供Serilog使用，同一個Id表示是同一個Request */
    public class CorrelationIdMiddleware
    {
        private readonly RequestDelegate _next;

        public CorrelationIdMiddleware(RequestDelegate next) 
        {
            _next = next;
        }

        public Task Invoke(HttpContext context)
        {
            var guid = Guid.NewGuid();
            using (LogContext.PushProperty("CorrelationId", guid.ToString()))
            {
                return _next.Invoke(context);
            }
        }
    }
}
