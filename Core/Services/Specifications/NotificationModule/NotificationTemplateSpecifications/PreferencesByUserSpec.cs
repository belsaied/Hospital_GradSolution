using Domain.Models.NotificationModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.NotificationModule.NotificationTemplateSpecifications
{
    public sealed class PreferencesByUserSpec : BaseSpecifications<NotificationPreference, int>
    {
        public PreferencesByUserSpec(string userId)
            : base(p => p.UserId == userId) { }
    }
}
