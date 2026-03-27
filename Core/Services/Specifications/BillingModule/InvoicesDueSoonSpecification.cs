using Domain.Models.BillingModule;
using Domain.Models.Enums.BillingEnums;

namespace Services.Specifications.BillingModule
{
    public sealed class InvoicesDueSoonSpecification : BaseSpecifications<Invoice,Guid>
    {
        public InvoicesDueSoonSpecification(int daysAhead)
    : base(i =>
        (i.Status == InvoiceStatus.Issued || i.Status == InvoiceStatus.PartiallyPaid)
        && i.DueDate.HasValue
        && i.DueDate.Value == DateOnly.FromDateTime(DateTime.UtcNow.AddDays(daysAhead)))
        { }
    }
}
