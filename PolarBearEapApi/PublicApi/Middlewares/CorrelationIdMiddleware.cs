using Serilog.Context;

namespace PolarBearEapApi.PublicApi.Middlewares
{
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
