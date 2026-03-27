using Domain.Models.BillingModule;
using Domain.Models.Enums.BillingEnums;

namespace Services.Specifications.BillingModule
{
    public sealed class OverdueInvoicesSpecification : BaseSpecifications<Invoice,Guid>
    {
        public OverdueInvoicesSpecification()
    : base(i =>
        (i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.PartiallyPaid)
        && i.DueDate.HasValue
        && i.DueDate.Value < DateOnly.FromDateTime(DateTime.UtcNow))
        { }
    }
}
