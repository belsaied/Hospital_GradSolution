using Domain.Models.Enums.NotificationEnums;
using Domain.Models.NotificationModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.NotificationModule.NotificationTemplateSpecifications
{
    public sealed class ActiveTemplateSpec : BaseSpecifications<NotificationTemplate, int>
    {
        public ActiveTemplateSpec(NotificationType type, NotificationChannel channel)
            : base(t =>
                t.NotificationType == type &&
                t.Channel == channel &&
                t.IsActive)
        { }
    }
}
