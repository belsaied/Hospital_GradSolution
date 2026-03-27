using Domain.Models.BillingModule;

namespace Services.Specifications.BillingModule
{
    public sealed class InvoiceWithDetailsSpecification : BaseSpecifications<Invoice,Guid>
    {
        public InvoiceWithDetailsSpecification(Guid invoiceId) : base(i => i.Id == invoiceId)
        {
            AddInclude(i => i.LineItems);
            AddInclude(i => i.Payments);
            AddInclude(i => i.InsuranceClaim!);
        }
    }
}
