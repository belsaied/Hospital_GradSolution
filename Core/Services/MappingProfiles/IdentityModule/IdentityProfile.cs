using AutoMapper;
using Domain.Models.IdentityModule;
using Shared.Dtos.UserManagementDtos;

namespace Services.MappingProfiles.IdentityModule
{
    public class IdentityProfile : Profile
    {
        public IdentityProfile()
        {
            CreateMap<ApplicationUser, UserInfoDto>()
    .ForMember(d => d.FullName, o => o.MapFrom(s => s.FullName));
        }
    }
}
