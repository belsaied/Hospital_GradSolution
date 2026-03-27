using Services.Abstraction.Contracts.BillingService;

namespace Presentation.Hubs
{
    public class InvoiceNotifier : IInvoiceNotifier
    {
        public Task NotifyInvoiceDueSoonAsync(int patientId, Guid invoiceId,
        string invoiceNumber, decimal outstandingBalance, DateOnly dueDate)
       => Task.CompletedTask;
    }
}
