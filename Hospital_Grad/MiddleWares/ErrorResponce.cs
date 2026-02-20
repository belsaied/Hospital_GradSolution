using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Responces
{
    public class ErrorResponce
    {
        public int StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string? TraceId { get; set; }
        public IDictionary<string, string[]>? Errors { get; set; }

        public ErrorResponce() 
        {
        }
        public ErrorResponce(int statusCode, string message, string? details = null)
        {
            StatusCode = statusCode;
            Message = message;
            Details = details;
            
        }
    }
}
