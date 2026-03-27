using Domain.Models.BillingModule;
using Shared.Parameters;

namespace Services.Specifications.BillingModule
{
    public sealed class InvoiceCountSpecification : BaseSpecifications<Invoice, Guid>
    {
        public InvoiceCountSpecification(InvoiceFilterParameters p)
    : base(i =>
        (!p.Status.HasValue || i.Status == p.Status.Value) &&
        (!p.PatientId.HasValue || i.PatientId == p.PatientId.Value))
        { }
    }
}
