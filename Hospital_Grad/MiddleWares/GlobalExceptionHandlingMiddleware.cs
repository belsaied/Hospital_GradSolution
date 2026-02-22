using Microsoft.AspNetCore.Mvc;
using Services.Exceptions;
using Shared.Responces;
using System;
using System.Net;
using System.Text.Json;

namespace Hospital_Grad.API.MiddleWares
{
    public class GlobalExceptionHandlingMiddleware(RequestDelegate _next, ILogger<GlobalExceptionHandlingMiddleware> _logger)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next.Invoke(context);
                await NotFoundEndPointException(context);
            }
            catch (Exception ex)
            {
                #region Logging The Error
                _logger.LogError(ex, "An unhandled exception occurred.");
                #endregion

                #region Return Custom Response
                var response = new ProblemDetails()
                { 
                 Title="An error occurred while processing your request.",
                 Detail= ex.Message,
                 Instance=context.Request.Path,
                 Status= ex switch
                 {
                     NotFoundException =>StatusCodes.Status404NotFound,
                     _ => StatusCodes.Status500InternalServerError
                 }
                };
                context.Response.StatusCode=response.Status.Value;
                await context.Response.WriteAsJsonAsync(response);
                #endregion
            }
        }

        private static async Task NotFoundEndPointException(HttpContext context)
        {
            if (context.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                var response = new ProblemDetails()
                {
                    Title = "Error while processing the HTTP Request -EndPoint is not found",
                    Detail = $"The requested EndPoint at {context.Request.Path} is not found.",
                    Instance = context.Request.Path,
                    Status = StatusCodes.Status404NotFound
                };
                await context.Response.WriteAsJsonAsync(response);
            }
        }


    }
}
