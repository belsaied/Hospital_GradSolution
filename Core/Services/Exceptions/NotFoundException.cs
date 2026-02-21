using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Exceptions
{
    public  class NotFoundException : Exception
    {
        public NotFoundException(string entityName, object Key):base($"{entityName} with Id {Key} Was not Found.") 
        {
        }
        public NotFoundException(string message):base(message)
        {
        }
        public NotFoundException(string entityName,object Key ,Exception exception) : base($"{entityName} With Id {Key} Was Not Found",exception)
        {
        }
    }
   
}
