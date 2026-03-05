using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.UserManagementDtos
{
    public record RegisterDto
    {
        [Required] public string FirstName { get; init; } = string.Empty;
        [Required] public string LastName { get; init; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; init; } = string.Empty;
        [Required] public string Password { get; init; } = string.Empty;
        [Required] public string Role { get; init; } = string.Empty;
        public int? DoctorId { get; init; }
        public int? PatientId { get; init; }

    }
}
