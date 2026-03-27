using Shared.Dtos.BillingModule.Results;
using Shared.Parameters;

namespace Services.Abstraction.Contracts.BillingService
{
    public interface IReportingService
    {
        Task<RevenueReportResultDto> GetRevenueReportAsync(ReportFilterParameters filters);
        Task<IEnumerable<InvoiceSummaryResultDto>> GetOutstandingInvoicesReportAsync();
        Task<byte[]> ExportRevenueToExcelAsync(ReportFilterParameters filters);
    }
}
