using AutoMapper;
using Domain.Models.DoctorModule;
using Shared.Dtos.DoctorModule.DoctorDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.MappingProfiles.DoctorModule
{
    public class ScheduleProfile :Profile
    {
        public ScheduleProfile()
        {
            CreateMap<CreateScheduleDto, DoctorSchedule>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DoctorId, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore());

            CreateMap<DoctorSchedule, ScheduleResultDto>()
                .ForMember(dest => dest.DayOfWeek, opt => opt.MapFrom(src => src.DayOfWeek.ToString()));

        }
    }
}
