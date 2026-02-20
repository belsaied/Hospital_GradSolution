using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Exceptions
{
    public class ConflictException : Exception
    {
        public string PropName { get; }
        public ConflictException(string propName, object value) : base($"A resource with {propName} '{value}' already exists")
        {
            PropName = propName;
        }
        public ConflictException(string message):base(message)
        {
            PropName = string.Empty;
        }
        public ConflictException(string propName, object value, Exception exception) : base($"A resource with {propName} '{value}' already exists",exception)
        { 
            PropName =propName;
        }
    }
}
