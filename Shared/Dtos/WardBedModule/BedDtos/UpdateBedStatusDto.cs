using Domain.Models.Enums.WardBedEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Dtos.WardBedModule.BedDtos
{
    public record UpdateBedStatusDto
    {
        [Required]
        public BedStatus Status { get; init; }

        [MaxLength(500)]
        public string? Notes { get; init; }
    }
}
