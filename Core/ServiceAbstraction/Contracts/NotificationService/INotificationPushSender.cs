using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Abstraction.Contracts.NotificationService
{
    public interface INotificationPushSender
    {
        Task SendAsync(string userId, object payload);
        Task<bool> IsUserConnectedAsync(string userId);
    }
}
