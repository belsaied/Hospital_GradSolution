using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts.BillingService;
using Shared.Dtos.BillingModule.Requests;
using Shared.Dtos.BillingModule.Results;

namespace Presentation.Controllers.BillingModule
{
    [Route("api/insurance")]
    public class InsuranceController(IInsuranceService _insuranceService) : ApiController
    {
        // POST /api/insurance/claims
        [HttpPost("claims")]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Receptionist")]
        [ProducesResponseType(typeof(ClaimResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<ClaimResultDto>> SubmitClaim([FromBody] SubmitClaimRequest request)
        {
            var claim = await _insuranceService.SubmitClaimAsync(request);
            return StatusCode(StatusCodes.Status201Created, claim);
        }

        // GET /api/insurance/claims/{invoiceId}
        [HttpGet("claims/{invoiceId:guid}")]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Receptionist")]
        [ProducesResponseType(typeof(ClaimResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ClaimResultDto>> GetClaimByInvoice(Guid invoiceId)
            => Ok(await _insuranceService.GetClaimByInvoiceAsync(invoiceId));

        // PUT /api/insurance/claims/{claimId}/status
        [HttpPut("claims/{claimId:guid}/status")]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
        [ProducesResponseType(typeof(ClaimResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<ClaimResultDto>> UpdateClaimStatus(
            Guid claimId, [FromBody] UpdateClaimRequest request)
            => Ok(await _insuranceService.UpdateClaimStatusAsync(claimId, request));

        // PUT /api/insurance/claims/{claimId}/resubmit
        [HttpPut("claims/{claimId:guid}/resubmit")]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Receptionist")]
        [ProducesResponseType(typeof(ClaimResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<ClaimResultDto>> ResubmitClaim(
            Guid claimId, [FromBody] ResubmitClaimRequest request)
            => Ok(await _insuranceService.ResubmitClaimAsync(claimId, request));
    }
}
