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
    public class ConflictException : Exception
    {
        public ConflictException(string message) 
            :base(message)  { }
    }
    public class ValidationException : Exception
    {
        public ValidationException(string message)
            : base(message) { }
    }
    #region Patient Module
    public sealed class PatientNotFoundException : NotFoundException
    {
        public PatientNotFoundException(int id)
            : base("Patient", id) { }
    }
    #endregion

    #region Doctor Module
    public sealed class DoctorNotFoundException : NotFoundException
    {
        public DoctorNotFoundException(int id)
            :base("Doctor",id) { }
    }

    public sealed class DepartmentNotFoundException : NotFoundException
    {
        public DepartmentNotFoundException(int id)
            :base("Department",id) { }
    }
    public sealed class QualificationNotFoundException : NotFoundException
    {
        public QualificationNotFoundException(int id)
            : base("Qualification", id) { }
    }

    public sealed class DuplicateLicenseNumberException : ConflictException
    {
        public DuplicateLicenseNumberException(string licenseNumber)
            : base($"A doctor with LicenseNumber '{licenseNumber}' already exists") { }
    }

    public sealed class DuplicateDoctorEmailException : ConflictException
    {
        public DuplicateDoctorEmailException(string email)
            : base($"A doctor with Email '{email}' already exists") { }
    }

    public sealed class InvalidScheduleTimeException : ValidationException
    {
        public InvalidScheduleTimeException()
            : base("EndTime must be strictly greater than StartTime") { }
    }

    public sealed class OverlappingScheduleException : ValidationException
    {
        public OverlappingScheduleException(string day)
            : base($"Doctor already has a schedule for {day}. Remove it first before setting a new one") { }
    }
    #endregion

    #region Appointment Module
    public sealed class AppointmentNotFoundException : NotFoundException
    {
        public AppointmentNotFoundException(int id)
            : base("Appointment", id) { }
    }
    #endregion

}
