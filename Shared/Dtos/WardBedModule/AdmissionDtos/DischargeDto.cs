using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Dtos.WardBedModule.AdmissionDtos
{
    public record DischargeDto
    {
        [MaxLength(2000)]
        public string? DischargeSummary { get; init; }
    }
}
