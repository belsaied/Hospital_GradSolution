namespace Services.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string entityName, object id)
            : base($"{entityName} with Id: {id} is not found") { }

        public NotFoundException(string message)
            : base(message) { }
    }

    public class BusinessRuleException : Exception
    {
        public BusinessRuleException(string message)
            : base(message) { }
    }

    public sealed class PatientNotFoundException : NotFoundException
    {
        public PatientNotFoundException(int id)
            : base("Patient", id) { }
    }

}
