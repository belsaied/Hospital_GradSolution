using Microsoft.AspNetCore.Mvc;
using Services.Exceptions;
using Shared.ErrorModels;

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
                await HandleForbiddenAsync(context);
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

        private static async Task HandleForbiddenAsync(HttpContext context)
        {
            if (context.Response.StatusCode == StatusCodes.Status403Forbidden
                && !context.Response.HasStarted)
            {
                var response = new ProblemDetails
                {
                    Title = "Forbidden",
                    Detail = "You do not have permission to access this resource.",
                    Instance = context.Request.Path,
                    Status = StatusCodes.Status403Forbidden
                };
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(response);
            }
        }
        private static async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            int statusCode;
            IEnumerable<string>? validationErrors = null;

            switch (ex)
            {
                case ValidationException ve:
                    statusCode = StatusCodes.Status400BadRequest;
                    validationErrors = ve.Errors;
                    break;
                case NotFoundException:
                    statusCode = StatusCodes.Status404NotFound;
                    break;
                case ForbiddenException:
                    statusCode = StatusCodes.Status403Forbidden;
                    break;
                case ConflictException:
                    statusCode = StatusCodes.Status409Conflict;
                    break;
                case AccountLockedException:
                case EmailNotVerifiedException:
                    statusCode = StatusCodes.Status403Forbidden;
                    break;
                case BusinessRuleException:
                    statusCode = StatusCodes.Status422UnprocessableEntity;
                    break;
                default:
                    statusCode = StatusCodes.Status500InternalServerError;
                    break;
            }

            var response = new ProblemDetails
            {
                Title = GetTitle(ex),
                Detail = ex.Message,
                Instance = context.Request.Path,
                Status = statusCode
            };

            if (validationErrors is not null)
                response.Extensions["errors"] = validationErrors;

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(response);
        }

        private static string GetTitle(Exception ex) => ex switch
        {
            //Updated =>Abdo
            NotFoundException => "Resource Not Found",
            ConflictException => "Confilct",
            ValidationException => "Validation Error",
            ForbiddenException => "Forbidden",
            BusinessRuleException => "Business Rule Violation",
            _ => "An unexpected error occurred"
        };

    }
}
