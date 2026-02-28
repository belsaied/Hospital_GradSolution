using AutoMapper;
using Domain.Models.MedicalRecordModule;
using Shared.Dtos.MedicalRecordsDto;

namespace Services.MappingProfiles.MedicalRecordModule
{
    public class LabResultProfile : Profile
    {
        public LabResultProfile()
        {
            CreateMap<CreateLabResultDto, LabResult>()
    .ForMember(d => d.Id, o => o.Ignore())
    .ForMember(d => d.LabOrderId, o => o.Ignore())
    .ForMember(d => d.LabOrder, o => o.Ignore());

            CreateMap<LabResult, LabResultResultDto>();
        }
    }
}
