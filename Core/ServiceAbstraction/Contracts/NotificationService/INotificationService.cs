using Domain.Models.Enums.NotificationEnums;
using Shared.Dtos.NotificationDtos.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Abstraction.Contracts.NotificationService
{
    public interface INotificationService
    {

        Task SendAppointmentConfirmedAsync(AppointmentNotificationEvent evt);
        Task SendAppointmentCancelledAsync(AppointmentNotificationEvent evt);
        Task SendAppointmentReminderAsync(int patientId, int appointmentId);
        Task SendAbnormalLabResultAsync(AbnormalLabResultEvent evt);
        Task SendPrescriptionExpiryWarningAsync(int patientId, int prescriptionId, DateOnly expiryDate);
        Task SendInvoiceIssuedAsync(InvoiceNotificationEvent evt);
        Task SendPaymentReceivedAsync(PaymentNotificationEvent evt);
        Task SendInvoiceOverdueAsync(InvoiceNotificationEvent evt);
        Task SendCustomAdminMessageAsync(int recipientId, RecipientType recipientType, string subject, string body, NotificationChannel channel);
    }
}
