using Domain.Models.BillingModule;
using Domain.Models.Enums.BillingEnums;

namespace Services.Specifications.BillingModule
{
    public sealed class InvoiceReportSpecification : BaseSpecifications<Invoice,Guid>
    {
        public InvoiceReportSpecification(DateOnly startDate, DateOnly endDate)
    : base(i =>
        i.CreatedAt >= startDate.ToDateTime(TimeOnly.MinValue).ToUniversalTime()
        && i.CreatedAt <= endDate.ToDateTime(TimeOnly.MaxValue).ToUniversalTime()
        && i.Status != InvoiceStatus.Cancelled)
        {
            AddInclude(i => i.LineItems);
            AddInclude(i => i.Payments);
        }
    }
}
