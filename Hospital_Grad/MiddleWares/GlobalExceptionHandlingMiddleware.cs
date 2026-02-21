using Microsoft.AspNetCore.Mvc;
using Services.Exceptions;
using Shared.Responces;
using System;
using System.Net;
using System.Text.Json;

namespace Hospital_Grad.API.MiddleWares
{
    public class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _environment;
        public GlobalExceptionHandlingMiddleware(RequestDelegate Next,ILogger<GlobalExceptionHandlingMiddleware> logger,
            IHostEnvironment environment) //  Ilogger => Identitfy Levels (Info, Warning, Error,..) in Console
        {
            _next = Next;
            _logger = logger;
            _environment = environment;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
                
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context,ex);
                
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            //Log The Exception 
            _logger.LogError(ex, "An unhandle exception occurred : {Message}", ex.Message);

            // Prepare error responce 
            var errorResponce = CreateErrorResponce(context, ex);
        
            //Set responce properties
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = errorResponce.StatusCode;

            // Serialize and write response
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            var jsonResponse = JsonSerializer.Serialize(errorResponce, jsonOptions);
            await context.Response.WriteAsync(jsonResponse);
        }

        private ErrorResponce CreateErrorResponce(HttpContext context, Exception ex)
        {
            var traceId = context.TraceIdentifier;
            return ex switch
            {
                //404 Not Found 
                NotFoundException notFoundException => new ErrorResponce
                {
                    StatusCode = (int)HttpStatusCode.NotFound,
                    Message = notFoundException.Message,
                    Details = _environment.IsDevelopment() ? notFoundException.StackTrace : null,
                    TraceId = traceId
                },
                // 400 - Bad Request (Validation)
                ValidationException validationEx => new ErrorResponce
                {
                    StatusCode = (int)HttpStatusCode.BadRequest,
                    Message = validationEx.Message,
                    Errors = validationEx.Errors,
                    Details = _environment.IsDevelopment() ? validationEx.StackTrace : null,
                    TraceId = traceId
                },
                // 409 - Conflict
                ConflictException conflictEx => new ErrorResponce
                {
                    StatusCode = (int)HttpStatusCode.Conflict,
                    Message = conflictEx.Message,
                    Details = _environment.IsDevelopment() ? conflictEx.StackTrace : null,
                    TraceId = traceId
                },
                // 422 - Unprocessable Entity (Business Rule)
                BusinessRuleException businessEx => new ErrorResponce
                {
                    StatusCode = (int)HttpStatusCode.UnprocessableEntity,
                    Message = businessEx.Message,
                    Details = _environment.IsDevelopment() ? businessEx.StackTrace : null,
                    TraceId = traceId
                },
                // 500 - Internal Server Error (Default)
                _ => new ErrorResponce
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = _environment.IsDevelopment()
                        ? ex.Message
                        : "An unexpected error occurred. Please try again later.",
                    Details = _environment.IsDevelopment() ? ex.StackTrace : null,
                    TraceId = traceId
                }
            };
        }
    }
}
