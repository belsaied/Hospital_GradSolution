using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts.BillingService;
using Shared.Dtos.BillingModule.Results;
using Shared.Parameters;

namespace Presentation.Controllers.BillingModule
{
    [Route("api/reports")]
    public class ReportsController (IReportingService _reportingService): ApiController
    {
        // GET /api/reports/revenue
        [HttpGet("revenue")]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
        [ProducesResponseType(typeof(RevenueReportResultDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<RevenueReportResultDto>> GetRevenueReport(
            [FromQuery] ReportFilterParameters filters)
            => Ok(await _reportingService.GetRevenueReportAsync(filters));

        // GET /api/reports/outstanding
        [HttpGet("outstanding")]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Receptionist")]
        [ProducesResponseType(typeof(IEnumerable<InvoiceSummaryResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InvoiceSummaryResultDto>>> GetOutstandingInvoices()
            => Ok(await _reportingService.GetOutstandingInvoicesReportAsync());

        // GET /api/reports/revenue/export
        [HttpGet("revenue/export")]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> ExportRevenueToExcel([FromQuery] ReportFilterParameters filters)
        {
            var bytes = await _reportingService.ExportRevenueToExcelAsync(filters);
            var fileName = $"revenue-report-{DateTime.UtcNow:yyyyMMdd}.xlsx";
            return File(bytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
    }
}
