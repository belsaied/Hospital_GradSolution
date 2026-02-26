using Domain.Models.Enums.AppointmentEnums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.AppointmentModule
{
    public record UpdateAppointmentStatusDto
    {
        [Required]
        public AppointmentStatus NewStatus { get; init; }

        [MaxLength(500)]
        public string? CancellationReason { get; init; }

        [MaxLength(2000)]
        public string? Notes { get; init; }
    }
}
