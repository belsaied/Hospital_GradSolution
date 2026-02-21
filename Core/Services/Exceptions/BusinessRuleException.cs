using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Exceptions
{
    public class BusinessRuleException :Exception
    {
        public string RuleName { get; }
        public BusinessRuleException(string ruleName,string message)  :base(message)
        {
            RuleName = ruleName;
        }
        public BusinessRuleException(string message) :base(message)
        {
            RuleName = "General  Business Rule";
        }
        public BusinessRuleException(string ruleName, string message, Exception exception) : base(message, exception)
        {
            RuleName = ruleName;
        }
    }
}
