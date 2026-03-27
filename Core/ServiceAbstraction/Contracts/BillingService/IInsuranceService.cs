using Shared.Dtos.BillingModule.Requests;
using Shared.Dtos.BillingModule.Results;
using Shared.Parameters;

namespace Services.Abstraction.Contracts.BillingService
{
    public interface IInsuranceService
    {
        Task<ClaimResultDto> SubmitClaimAsync(SubmitClaimRequest request);
        Task<ClaimResultDto> GetClaimByInvoiceAsync(Guid invoiceId);
        Task<ClaimResultDto> UpdateClaimStatusAsync(Guid claimId, UpdateClaimRequest request);
        Task<ClaimResultDto> ResubmitClaimAsync(Guid claimId, ResubmitClaimRequest request);
    }

}
