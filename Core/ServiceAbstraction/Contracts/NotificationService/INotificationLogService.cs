using Shared;
using Shared.Dtos.NotificationDtos.Requests;
using Shared.Dtos.NotificationDtos.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Abstraction.Contracts.NotificationService
{
    public interface INotificationLogService
    {
        Task<PaginatedResult<NotificationLogResult>> GetNotificationsByUserAsync(string userId, NotificationLogFilter filter);
        Task<int> GetUnreadPushCountAsync(string userId);
        Task MarkPushNotificationReadAsync(Guid notificationId);
        Task MarkAllPushNotificationsReadAsync(string userId);
        Task<IEnumerable<UnreadPushNotificationResult>> GetUnreadPushNotificationsAsync(string userId);
    }
}
