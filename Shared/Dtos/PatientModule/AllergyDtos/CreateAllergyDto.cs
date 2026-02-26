using Domain.Models.Enums.PatientEnums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.PatientModule.AllergyDtos
{
    public record CreateAllergyDto
    {
        [Required]
        public AllergyType Type { get; init; }

        [Required]
        public Severity Severity { get; init; }

        [Required, MaxLength(1000)]
        public string Description { get; init; } = string.Empty;

        [Required, MaxLength(100)]
        public string RecordedBy { get; init; } = string.Empty;
    }
}
