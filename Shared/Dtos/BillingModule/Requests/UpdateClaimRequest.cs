using Domain.Models.Enums.BillingEnums;
using System.ComponentModel.DataAnnotations;

namespace Shared.Dtos.BillingModule.Requests
{
    public record UpdateClaimRequest
    {
        [Required]
        public ClaimStatus ClaimStatus { get; init; }

        [Range(0, double.MaxValue)]
        public decimal? ApprovedAmount { get; init; }

        [MaxLength(500)]
        public string? RejectionReason { get; init; }
    }
}
