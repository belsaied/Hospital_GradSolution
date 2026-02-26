using Domain.Models.Enums.PatientEnums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.PatientModule.PatientDtos
{
    public record CreatePatientDto
    {
        [Required, MaxLength(100)]
        public string FirstName { get; init; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; init; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; init; }

        [Required]
        public Gender Gender { get; init; }

        public BloodType? BloodType { get; init; }

        [Required, MaxLength(50)]
        public string NationalId { get; init; } = string.Empty;

        [Required, Phone]
        public string Phone { get; init; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; init; } = string.Empty;

        [Required]
        public AddressDto Address { get; init; } = null!;
        public string? PictureUrl { get; init; }
    }
}
