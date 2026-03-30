using Domain.Models.Enums.NotificationEnums;
using Domain.Models.NotificationModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.NotificationModule.NotificationTemplateSpecifications
{
    public sealed class PreferenceByUserTypeChannelSpec : BaseSpecifications<NotificationPreference, int>
    {
        public PreferenceByUserTypeChannelSpec(string userId, NotificationType type, NotificationChannel channel)
            : base(p =>
                p.UserId == userId &&
                p.NotificationType == type &&
                p.Channel == channel)
        { }
    }
}
