using Microsoft.AspNetCore.SignalR;
using Services.Abstraction.Contracts.WardBedService;
using System;
using System.Collections.Generic;
using System.Text;

namespace Presentation.Hubs
{
    public class BedNotifier (IHubContext<WardHub> _hubContext) :IBedNotifier
    {
        public async Task NotifyWardAsync(int wardId, string eventName, object payload)
        {
            await _hubContext.Clients
                .Group($"ward-{wardId}")
                .SendAsync(eventName, payload);
        }

        public async Task NotifyDashboardAsync(string eventName, object payload)
        {
            await _hubContext.Clients
                .Group("dashboard")
                .SendAsync(eventName, payload);
        }
    }
}
