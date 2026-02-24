using AutoMapper;
using Domain.Models.DoctorModule;
using Shared.Dtos.DoctorModule.DepartmentDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.MappingProfiles.DepartmentModule
{
    public class DepartmentProfile :Profile
    {
        public DepartmentProfile()
        {
            // Mapping CreateDepartmentDto -> Department
            CreateMap<CreateDepartmentDto, Department>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.HeadDoctorId, opt => opt.Ignore())
                .ForMember(dest => dest.HeadDoctor, opt => opt.Ignore())
                .ForMember(dest => dest.Doctors, opt => opt.Ignore());

            //Mapping Department -> DepartmentResultDto
            CreateMap<Department, DepartmentResultDto>()
                .ForMember(dest => dest.HeadDoctorName, opt => opt.MapFrom(src => src.HeadDoctor != null ? $"{src.HeadDoctor.FirstName} {src.HeadDoctor.LastName}" : null));
        }
    }
}
