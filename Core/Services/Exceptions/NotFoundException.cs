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
        public IEnumerable<string> Errors { get; set; } = [];

        public ValidationException(IEnumerable<string> errors) : base("Validation Failed")
        {
            Errors = errors;
        }
    }
    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }
    public sealed class AccountLockedException : Exception
    {
        public AccountLockedException(DateTime lockoutEnd)
    : base($"Account locked until {lockoutEnd:u}") { }
    }

    public sealed class EmailNotVerifiedException : Exception
    {
        public EmailNotVerifiedException()
            : base("Please verify your email address before logging in.") { }
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
            : base(new[] { "EndTime must be strictly greater than StartTime" }) { }
    }

    public sealed class OverlappingScheduleException : ValidationException
    {
        public OverlappingScheduleException(string day)
            : base(new[] { $"Doctor already has a schedule for {day}. Remove it first before setting a new one" }) { }
    }
    #endregion

    #region Appointment Module
    public sealed class AppointmentNotFoundException : NotFoundException
    {
        public AppointmentNotFoundException(int id)
            : base("Appointment", id) { }
    }
    #endregion

    #region Medical Record Module

    public sealed class MedicalRecordNotFoundException : NotFoundException
    {
        public MedicalRecordNotFoundException(int id)
            : base("MedicalRecord", id) { }
    }

    public sealed class LabOrderNotFoundException : NotFoundException
    {
        public LabOrderNotFoundException(int id)
            : base("LabOrder", id) { }
    }

    public sealed class PrescriptionNotFoundException : NotFoundException
    {
        public PrescriptionNotFoundException(int id)
            : base("Prescription", id) { }
    }

    #endregion

    #region WardBedModule
    public class WardNotFoundException(int id)
        : NotFoundException($"Ward with ID {id} was not found.");

    public class RoomNotFoundException(int id)
        : NotFoundException($"Room with ID {id} was not found.");

    public class BedNotFoundException(int id)
        : NotFoundException($"Bed with ID {id} was not found.");

    public class AdmissionNotFoundException(int id)
        : NotFoundException($"Admission with ID {id} was not found.");

    public class DuplicateWardNameException(string name)
        : ConflictException($"A ward with the name '{name}' already exists.");
    #endregion

}
