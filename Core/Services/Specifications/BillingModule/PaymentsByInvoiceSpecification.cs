using Domain.Models.BillingModule;

namespace Services.Specifications.BillingModule
{
    public sealed class PaymentsByInvoiceSpecification : BaseSpecifications<Payment,Guid>
    {
        public PaymentsByInvoiceSpecification(Guid invoiceId)
    : base(p => p.InvoiceId == invoiceId)
        {
            AddOrderByDescending(p => p.CreatedAt);
        }
    }
}
