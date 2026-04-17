using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.UserManagementDtos
{
    public record RegisterDto
    {
        [Required, MaxLength(100, ErrorMessage = "FirstName cannot exceed 100 characters.")]
        public string FirstName { get; init; } = string.Empty;

        [Required, MaxLength(100, ErrorMessage = "LastName cannot exceed 100 characters.")]
        public string LastName { get; init; } = string.Empty;

        [Required, EmailAddress, MaxLength(256)]
        public string Email { get; init; } = string.Empty;

        [Required, MinLength(8), MaxLength(100)]
        public string Password { get; init; } = string.Empty;

        [Required]
        [RegularExpression("^(Patient|Doctor|Nurse|Receptionist|HospitalAdmin|SuperAdmin)$",
            ErrorMessage = "Role must be one of: Patient, Doctor, Nurse, Receptionist, HospitalAdmin, SuperAdmin.")]
        public string Role { get; init; } = string.Empty;

        public int? DoctorId { get; init; }
        public int? PatientId { get; init; }

    }
}
