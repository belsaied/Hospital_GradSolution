using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Presentation.Hubs
{
    public class WardHub :Hub
    {
        public async Task JoinWard(int wardId)
            => await Groups.AddToGroupAsync(Context.ConnectionId, $"ward-{wardId}");

        public async Task JoinDashboard()
            => await Groups.AddToGroupAsync(Context.ConnectionId, "dashboard");
    }
}
