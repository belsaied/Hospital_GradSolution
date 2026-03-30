using AutoMapper;
using Domain.Contracts;
using Domain.Models.Enums.NotificationEnums;
using Domain.Models.NotificationModule;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Services.Abstraction.Contracts.NotificationService;
using Services.Specifications.NotificationModule;
using Services.Specifications.NotificationModule.NotificationSpecification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Presentation.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task JoinUserGroup(string userId)
            => await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");

        public async Task LeaveUserGroup(string userId)
            => await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
    }
}
