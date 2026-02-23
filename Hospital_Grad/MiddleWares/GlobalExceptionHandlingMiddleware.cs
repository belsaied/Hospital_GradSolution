using Microsoft.AspNetCore.Mvc;
using Services.Exceptions;

namespace Hospital_Grad.API.MiddleWares
{
    public class GlobalExceptionHandlingMiddleware(
        RequestDelegate _next,
        ILogger<GlobalExceptionHandlingMiddleware> _logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
                await HandleNotFoundEndpointAsync(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred.");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleNotFoundEndpointAsync(HttpContext context)
        {
            if (context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                var response = new ProblemDetails
                {
                    Title = "Endpoint Not Found",
                    Detail = $"The requested endpoint at {context.Request.Path} was not found.",
                    Instance = context.Request.Path,
                    Status = StatusCodes.Status404NotFound
                };
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(response);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var statusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                BusinessRuleException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            var response = new ProblemDetails
            {
                Title = GetTitle(ex),
                Detail = ex.Message,
                Instance = context.Request.Path,
                Status = statusCode
            };

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }

        private static string GetTitle(Exception ex) => ex switch
        {
            NotFoundException => "Resource Not Found",
            BusinessRuleException => "Business Rule Violation",
            _ => "An unexpected error occurred"
        };
    }
}
