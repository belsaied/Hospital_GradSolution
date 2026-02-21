using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Exceptions
{
    public class ValidationException :Exception
    {
        public IDictionary<string, string[]> Errors { get; }
        public ValidationException() :base("One or more validation failures have occurred")
        {
            Errors = new Dictionary<string, string[]>();        
        }
        public ValidationException(IDictionary<string, string[]> errors) : base("One or More Validation Failures have occurred")
        {
            Errors = errors;
        }
        public ValidationException(string propName, string errorMessage) : base("One or More Validation Failures have occurred ")
        {
            Errors = new Dictionary<string, string[]>
            {
                {propName, new []{ errorMessage} }
            };
        }
    }
}
