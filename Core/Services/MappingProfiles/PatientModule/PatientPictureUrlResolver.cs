using AutoMapper;
using Domain.Models.PatientModule;
using Microsoft.Extensions.Configuration;
using Shared.Dtos.PatientModule.PatientDtos;

namespace Services.MappingProfiles.PatientModule
{
    public class PatientPictureUrlResolver<TDestination>(IConfiguration _configuration)
        : IValueResolver<Patient, TDestination, string?>

    {
        public string? Resolve(Patient source, TDestination destination, string? destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.PictureUrl))
                return null;

            return $"{_configuration.GetSection("URLS")["BaseUrl"]}{source.PictureUrl}";
        }
    }
}
