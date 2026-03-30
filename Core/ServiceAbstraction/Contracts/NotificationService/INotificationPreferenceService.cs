using Shared.Dtos.NotificationDtos.Requests;
using Shared.Dtos.NotificationDtos.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Abstraction.Contracts.NotificationService
{
    public interface INotificationPreferenceService
    {
        Task<IEnumerable<NotificationPreferenceResult>> GetPreferencesAsync(string userId);
        Task<NotificationPreferenceResult> UpdatePreferenceAsync(string userId, UpdatePreferenceRequest request);
        Task ResetPreferencesToDefaultAsync(string userId);
    }
}
