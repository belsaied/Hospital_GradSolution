using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Abstraction.Contracts.WardBedService
{
    public interface IBedNotifier
    {
        Task NotifyWardAsync(int wardId, string eventName, object payload);
        Task NotifyDashboardAsync(string eventName, object payload);

    }
}
