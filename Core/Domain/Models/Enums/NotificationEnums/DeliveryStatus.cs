using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Enums.NotificationEnums
{
    public enum DeliveryStatus
    {
        Pending = 0,
        Sent = 1,
        Failed = 2,
        Skipped = 3
    }
}
