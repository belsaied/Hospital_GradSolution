using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Abstraction.Contracts.NotificationService
{
    public interface ISmsSender
    {
        Task<string?> SendAsync(string toPhone, string body);
    }
}
