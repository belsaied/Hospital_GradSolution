using Domain.Models.Enums.PatientEnums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.DoctorModule.DoctorDtos
{
    public record CreateDoctorDto
    {
        [Required,MaxLength(100)]
        public string FirstName { get; init; } =string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; init; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; init; }

        [Required]
        public Gender Gender { get; init; }

        [Required,MaxLength(50)]
        public string NationalId { get; init; } = string.Empty;
        
        [Required, MaxLength(50)]
        public string LicenseNumber { get; init; } = string.Empty;

        [Required,EmailAddress]
        public string Email { get; init; } = string.Empty;

        [Required,Phone]
        public string Phone { get; init; } = string.Empty;

        [Required,MaxLength(100)]
        public string Specialization { get; init; } = string.Empty;

        [Required]
        public int DepartmentId { get; init; }

        [Required,Range(0,60)]
        public int YearOfExperience { get; init; }

        [Required,Range(0,100000)]
        public decimal ConsultationFee { get; init; }
        public string? PictureUrl { get; init; }
        [MaxLength(1000)]
        public string? Bio { get; init; }
    }
}
