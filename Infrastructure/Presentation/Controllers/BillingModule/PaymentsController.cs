using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts.BillingService;
using Shared.Dtos.BillingModule.Requests;
using Shared.Dtos.BillingModule.Results;

namespace Presentation.Controllers.BillingModule
{
    [Route("api/payments")]
    public class PaymentsController(IPaymentService _paymentService) : ApiController
    {
        // POST /api/payments/intent/{invoiceId}
        [HttpPost("intent/{invoiceId:guid}")]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Receptionist,Patient")]
        [ProducesResponseType(typeof(PaymentIntentResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<PaymentIntentResultDto>> CreatePaymentIntent(Guid invoiceId)
            => Ok(await _paymentService.CreatePaymentIntentAsync(invoiceId));

        // POST /api/payments/cash/{invoiceId}
        [HttpPost("cash/{invoiceId:guid}")]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Receptionist")]
        [ProducesResponseType(typeof(PaymentResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<PaymentResultDto>> RecordCashPayment(
            Guid invoiceId, [FromBody] RecordCashPaymentRequest request)
        {
            var payment = await _paymentService.RecordCashPaymentAsync(invoiceId, request.Amount);
            return StatusCode(StatusCodes.Status201Created, payment);
        }

        // POST /api/payments/webhook — AllowAnonymous; validated by Stripe-Signature header
        [HttpPost("webhook")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StripeWebhook()
        {
            // Raw body must be read BEFORE any model binding — Stripe validates exact bytes
            using var reader = new StreamReader(HttpContext.Request.Body);
            var payload = await reader.ReadToEndAsync();

            var signature = Request.Headers["Stripe-Signature"].FirstOrDefault();
            if (string.IsNullOrEmpty(signature))
                return BadRequest("Missing Stripe-Signature header.");

            await _paymentService.HandleStripeWebhookAsync(payload, signature);
            return Ok();
        }

        // POST /api/payments/{paymentId}/refund
        [HttpPost("{paymentId:guid}/refund")]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
        [ProducesResponseType(typeof(PaymentResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<PaymentResultDto>> RefundPayment(
            Guid paymentId, [FromBody] RefundRequest request)
            => Ok(await _paymentService.RefundPaymentAsync(paymentId, request.Amount, request.Reason));
    }
}
