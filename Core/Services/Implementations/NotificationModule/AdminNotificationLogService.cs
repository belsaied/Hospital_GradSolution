using AutoMapper;
using Domain.Contracts;
using Domain.Models.NotificationModule;
using Services.Abstraction.Contracts.NotificationService;
using Services.Specifications.NotificationModule.NotificationSpecification;
using Shared;
using Shared.Dtos.NotificationDtos.Requests;
using Shared.Dtos.NotificationDtos.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Implementations.NotificationModule
{
    public sealed class AdminNotificationLogService(
        IUnitOfWork _unitOfWork,
        IMapper _mapper) : IAdminNotificationLogService
    {
        public async Task<PaginatedResult<NotificationLogResult>> GetAllNotificationsAsync(
            NotificationLogFilter filter)
        {
            var repo = _unitOfWork.GetRepository<Notification, Guid>();
            var notifications = await repo.GetAllAsync(new AdminNotificationLogSpec(filter));
            var count = await repo.CountAsync(new AdminNotificationLogCountSpec(filter));

            return new PaginatedResult<NotificationLogResult>(
                filter.PageIndex, filter.PageSize, count,
                _mapper.Map<IEnumerable<NotificationLogResult>>(notifications));
        }
    }
}
