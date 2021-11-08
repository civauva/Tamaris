using System;
using System.Net;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;

using Tamaris.API.Infrastructure.Logging;


namespace Tamaris.API.Infrastructure.ExceptionMiddleware
{
	public class ExceptionMiddleware
	{
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                LogFrog.Error(ex, $"An unexpected fault happened. Either try again later, or contact the support.");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {  
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            return context.Response.WriteAsync(new ErrorDetails()
            {
                StatusCode = context.Response.StatusCode,
                Message = "Internal Server Error happened. If the error persists contact the support."
            }
            .ToString());
        }
    }
}