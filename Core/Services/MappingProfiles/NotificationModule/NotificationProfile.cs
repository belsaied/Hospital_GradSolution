using AutoMapper;
using Domain.Models.NotificationModule;
using Shared.Dtos.NotificationDtos.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.MappingProfiles.NotificationModule
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationLogResult>();

            CreateMap<Notification, UnreadPushNotificationResult>();

            CreateMap<NotificationPreference, NotificationPreferenceResult>();
        }
    }
}
