using AutoMapper;
using Domain.Models.DoctorModule;
using Shared.Dtos.DoctorModule.DoctorDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.MappingProfiles.DoctorModule
{
    public class QualificationProfile :Profile
    {
        public QualificationProfile()
        {
            CreateMap<CreateQualificationDto, DoctorQualification>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.DoctorId, opt => opt.Ignore())
                .ForMember(dest => dest.Doctor, opt => opt.Ignore());


            CreateMap<DoctorQualification, QualificationResultDto>();
        }
    }
}
