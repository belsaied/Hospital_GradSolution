using Domain.Models.BillingModule;
using Domain.Models.Enums.BillingEnums;

namespace Services.Specifications.BillingModule
{
    public sealed class InvoicesByStatusSpecification : BaseSpecifications<Invoice,Guid>
    {
        public InvoicesByStatusSpecification(IEnumerable<InvoiceStatus> statuses)
    : base(i => statuses.Contains(i.Status))
        {
            AddOrderByDescending(i => i.CreatedAt);
        }
    }
}
