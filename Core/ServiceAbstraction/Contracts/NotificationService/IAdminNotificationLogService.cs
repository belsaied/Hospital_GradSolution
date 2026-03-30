using Shared;
using Shared.Dtos.NotificationDtos.Requests;
using Shared.Dtos.NotificationDtos.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Abstraction.Contracts.NotificationService
{
    public interface IAdminNotificationLogService
    {
        Task<PaginatedResult<NotificationLogResult>> GetAllNotificationsAsync(NotificationLogFilter filter);
    }
}
