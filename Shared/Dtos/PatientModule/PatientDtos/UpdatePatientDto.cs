using Domain.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.PatientModule.PatientDtos
{
    public record UpdatePatientDto
    {
        [MaxLength(100)]
        public string? FirstName { get; init; }

        [MaxLength(100)]
        public string? LastName { get; init; }

        [Phone]
        public string? Phone { get; init; }

        [EmailAddress]
        public string? Email { get; init; }

        public AddressDto? Address { get; init; }

        public PatientStatus? Status { get; init; }
    }
}
