using AutoMapper;
using Domain.Models.Enums.WardBedEnums;
using Domain.Models.WardBedModule;
using Shared.Dtos.WardBedModule.RoomsDtos;
using Shared.Dtos.WardBedModule.WardDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.MappingProfiles.WardBedModule
{
    public class WardProfile :Profile
    {
        public WardProfile()
        {
            CreateMap<CreateWardDto,Ward>()
                .ForMember(d=>d.Id ,o=>o.Ignore())
                .ForMember(d => d.IsActive, o => o.Ignore())
                .ForMember(d => d.Rooms, o => o.Ignore());

            CreateMap<Ward, WardResultDto>()
                .ForMember(d => d.WardType, o => o.MapFrom(s => s.WardType.ToString()));

            CreateMap<CreateRoomDto, Room>()
                .ForMember(d => d.Id, o => o.Ignore())
                .ForMember(d => d.WardId, o => o.Ignore())
                .ForMember(d => d.IsActive, o => o.Ignore())
                .ForMember(d => d.Ward, o => o.Ignore())
                .ForMember(d => d.Beds, o => o.Ignore());

            CreateMap<Room, RoomResultDto>()
                .ForMember(d => d.WardName, o => o.MapFrom(s => s.Ward != null ? s.Ward.Name : string.Empty))
                .ForMember(d => d.RoomType, o => o.MapFrom(s => s.RoomType.ToString()));

            // WardProfile.cs — بدل الـ Ward → WardOccupancySummaryDto الطويل
            CreateMap<Ward, WardOccupancySummaryDto>()
                .ForMember(d => d.WardId, o => o.MapFrom(s => s.Id))
                .ForMember(d => d.TotalBeds, o => o.Ignore())
                .ForMember(d => d.OccupiedBeds, o => o.Ignore())
                .ForMember(d => d.AvailableBeds, o => o.Ignore())
                .ForMember(d => d.MaintenanceBeds, o => o.Ignore())
                .ForMember(d => d.ReservedBeds, o => o.Ignore());
                
        }
    }
}
