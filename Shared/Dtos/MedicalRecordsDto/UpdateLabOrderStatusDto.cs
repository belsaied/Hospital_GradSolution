using Domain.Models.Enums.MedicalRecordEnums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.MedicalRecordsDto
{
    public record UpdateLabOrderStatusDto
    {
        [Required]
        public LabOrderStatus NewStatus { get; init; }
    }
}
