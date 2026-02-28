using AutoMapper;
using Domain.Models.MedicalRecordModule;
using Shared.Dtos.MedicalRecordsDto;

namespace Services.MappingProfiles.MedicalRecordModule
{
    public class VitalSignProfile : Profile
    {
        public VitalSignProfile()
        {
            CreateMap<CreateVitalSignDto, VitalSign>()
    .ForMember(d => d.Id, o => o.Ignore())
    .ForMember(d => d.MedicalRecordId, o => o.Ignore())
    .ForMember(d => d.PatientId, o => o.Ignore())
    .ForMember(d => d.RecordedAt, o => o.Ignore())
    .ForMember(d => d.MedicalRecord, o => o.Ignore())
    .ForMember(d => d.Patient, o => o.Ignore());

            CreateMap<VitalSign, VitalSignResultDto>();
        }
    }
}
