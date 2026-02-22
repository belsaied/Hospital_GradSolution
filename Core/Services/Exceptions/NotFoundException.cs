using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Exceptions
{
    public abstract class NotFoundException(string Message) : Exception(Message);
    
    public sealed class PatientNotFoundException(int Id):NotFoundException($"Patient with Id: {Id} is not found");
   
}
