using Domain.Models.Enums.NotificationEnums;
using Domain.Models.NotificationModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.NotificationModule.NotificationSpecification
{
    public sealed class NotificationsByStatusSpec : BaseSpecifications<Notification, Guid>
    {
        public NotificationsByStatusSpec(string userId, DeliveryStatus status)
            : base(n => n.DeliveryStatus == status)
        {
            AddOrderByDescending(n => n.CreatedAt);
        }
    }
}
