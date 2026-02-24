using AutoMapper;
using Domain.Models.DoctorModule;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Services.MappingProfiles.DoctorModule
{
    public class DoctorPictureUrlResolver<TDestination>(IConfiguration _configuration)
        : IValueResolver<Doctor, TDestination, string?>
    {
        public string? Resolve(Doctor source, TDestination destination, string? destMember, ResolutionContext context)
        {
            if (string.IsNullOrEmpty(source.PictureUrl))
                return null;

            return $"{_configuration.GetSection("URLS")["BaseUrl"]}{source.PictureUrl}";
        }
    }

}
