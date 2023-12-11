namespace OddsFeed.Core
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using OddsFeed.Core.Services;

    public class ErrorHandler
    {
        private readonly RequestDelegate next;
        private readonly ILogServiceProvider logger;

        public ErrorHandler(RequestDelegate next, ILogServiceProvider logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleException(context, ex);
            }
        }

        private async Task HandleException(HttpContext context, Exception exception)
        {
            await logger.LogSentryAsync(exception);

            if (IsAjaxRequest(context.Request))
            {
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            }
            else
            {
                context.Response.Redirect("/errors/index");
            }
        }

        private static bool IsAjaxRequest(HttpRequest request)
           => string.Equals(request.Query["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase) ||
              string.Equals(request.Headers["X-Requested-With"], "XMLHttpRequest", StringComparison.OrdinalIgnoreCase);
    }
}