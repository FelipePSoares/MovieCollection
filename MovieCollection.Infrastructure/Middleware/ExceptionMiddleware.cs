using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace MovieCollection.Infrastructure.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly IHostEnvironment environment;
        private readonly ILogger logger;

        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment environment, ILogger logger)
        {
            this.next = next;
            this.environment = environment;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message);

                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.ContentType = "application/json";
            httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                Message = ex.Message,
                StackTrace = this.environment.IsDevelopment() ? ex.StackTrace?.ToString() : "Internal Server Error"
            };

            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(response, options);

            await httpContext.Response.WriteAsync(json);
        }
    }

    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(
            this IApplicationBuilder builder, ILogger logger)
        {
            return builder.UseMiddleware<ExceptionMiddleware>(logger);
        }
    }
}
