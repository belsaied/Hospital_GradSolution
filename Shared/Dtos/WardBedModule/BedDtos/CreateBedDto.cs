using Domain.Models.Enums.WardBedEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Dtos.WardBedModule.BedDtos
{
    public record CreateBedDto
    {
        [Required, MaxLength(20)]
        public string BedNumber { get; init; } = string.Empty;

        [Required]
        public BedType BedType { get; init; }

        [MaxLength(500)]
        public string? Notes { get; init; }
    }
}
