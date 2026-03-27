using Domain.Models.BillingModule;

namespace Services.Specifications.BillingModule
{
    public sealed class PaymentByStripeIntentSpecification : BaseSpecifications<Payment,Guid>
    {
        public PaymentByStripeIntentSpecification(string paymentIntentId)
    : base(p => p.StripePaymentIntentId == paymentIntentId)
        {
            AddInclude(p => p.Invoice);
        }
    }
}
