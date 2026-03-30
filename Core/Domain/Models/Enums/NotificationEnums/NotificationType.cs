using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Enums.NotificationEnums
{
    public enum NotificationType
    {
        AppointmentConfirmed = 0,
        AppointmentCancelled = 1,
        AppointmentReminder = 2,
        AbnormalLabResult = 3,
        PrescriptionExpiryWarning = 4,
        InvoiceIssued = 5,
        PaymentReceived = 6,
        InvoiceOverdue = 7,
        CustomAdminMessage = 8
    }
}
