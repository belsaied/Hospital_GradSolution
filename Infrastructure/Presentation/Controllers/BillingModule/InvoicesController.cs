using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Authorization;
using Services.Abstraction.Contracts.BillingService;
using Shared;
using Shared.Dtos.BillingModule.Requests;
using Shared.Dtos.BillingModule.Results;
using Shared.Parameters;

namespace Presentation.Controllers.BillingModule
{
    [Route("api/invoices")]
    public class InvoicesController(IInvoiceService _invoiceService) : ApiController
    {
        // POST /api/invoices
        [HttpPost]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Receptionist")]
        [ProducesResponseType(typeof(InvoiceResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<InvoiceResultDto>> CreateInvoice([FromBody] CreateInvoiceRequest request)
        {
            var invoice = await _invoiceService.CreateInvoiceAsync(request);
            return CreatedAtAction(nameof(GetInvoiceById), new { id = invoice.Id }, invoice);
        }

        // GET /api/invoices/{id}
        [HttpGet("{id:guid}")]
        [RedisCache(durationInSeconds: 300)]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Doctor,Receptionist,Patient")]
        [ProducesResponseType(typeof(InvoiceResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<InvoiceResultDto>> GetInvoiceById(Guid id)
            => Ok(await _invoiceService.GetInvoiceByIdAsync(id));

        // GET /api/invoices
        [HttpGet]
        [RedisCache(durationInSeconds: 300)]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Receptionist")]
        [ProducesResponseType(typeof(PaginatedResult<InvoiceSummaryResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<InvoiceSummaryResultDto>>> GetAllInvoices(
            [FromQuery] InvoiceFilterParameters filters)
            => Ok(await _invoiceService.GetAllInvoicesAsync(filters));

        // GET /api/invoices/patient/{patientId}
        [HttpGet("patient/{patientId:int}")]
        [RedisCache(durationInSeconds: 300)]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Doctor,Receptionist")]
        [ProducesResponseType(typeof(IEnumerable<InvoiceSummaryResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<InvoiceSummaryResultDto>>> GetInvoicesByPatient(int patientId)
            => Ok(await _invoiceService.GetInvoicesByPatientAsync(patientId));

        // POST /api/invoices/{id}/line-items
        [HttpPost("{id:guid}/line-items")]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Receptionist")]
        [ProducesResponseType(typeof(InvoiceResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<InvoiceResultDto>> AddLineItem(
            Guid id, [FromBody] AddLineItemRequest request)
            => Ok(await _invoiceService.AddLineItemAsync(id, request));

        // DELETE /api/invoices/{id}/line-items/{itemId}
        [HttpDelete("{id:guid}/line-items/{itemId:guid}")]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Receptionist")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> RemoveLineItem(Guid id, Guid itemId)
        {
            await _invoiceService.RemoveLineItemAsync(id, itemId);
            return NoContent();
        }

        // PUT /api/invoices/{id}/issue
        [HttpPut("{id:guid}/issue")]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Receptionist")]
        [ProducesResponseType(typeof(InvoiceResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<InvoiceResultDto>> IssueInvoice(
            Guid id, [FromBody] IssueInvoiceRequest request)
            => Ok(await _invoiceService.IssueInvoiceAsync(id, request));

        // PUT /api/invoices/{id}/cancel
        [HttpPut("{id:guid}/cancel")]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CancelInvoice(Guid id, [FromBody] CancelInvoiceRequest request)
        {
            await _invoiceService.CancelInvoiceAsync(id, request.Reason);
            return NoContent();
        }

        // GET /api/invoices/{id}/pdf
        [HttpGet("{id:guid}/pdf")]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Doctor,Receptionist,Patient")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadPdf(Guid id)
        {
            var bytes = await _invoiceService.GenerateInvoicePdfAsync(id);
            return File(bytes, "application/pdf", $"invoice-{id}.pdf");
        }
    }
}
