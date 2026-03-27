using Domain.Models.BillingModule;

namespace Services.Abstraction.Contracts.BillingService
{
    public interface IInvoicePdfGenerator
    {
        byte[] Generate(Invoice invoice, string patientName, string patientEmail);
    }
}
