namespace NetCore.API
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;
        public CustomMiddleware(RequestDelegate next) {
            _next = next;
        }
        public Task Invoke(HttpContext context) {
            //return context.Response.WriteAsync("Hello world");
            context.Response.Headers.Add("hackerby", "NDT");
            return _next(context);
        }
    }
}
