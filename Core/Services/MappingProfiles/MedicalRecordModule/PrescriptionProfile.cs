using AutoMapper;
using Domain.Models.MedicalRecordModule;
using Shared.Dtos.MedicalRecordsDto;

namespace Services.MappingProfiles.MedicalRecordModule
{
    public class PrescriptionProfile : Profile
    {
        public PrescriptionProfile()
        {
            CreateMap<CreatePrescriptionDto, Prescription>()
    .ForMember(d => d.Id, o => o.Ignore())
    .ForMember(d => d.MedicalRecordId, o => o.Ignore())
    .ForMember(d => d.PatientId, o => o.Ignore())
    .ForMember(d => d.DoctorId, o => o.Ignore())
    .ForMember(d => d.PrescribedAt, o => o.Ignore())
    .ForMember(d => d.Status, o => o.Ignore())
    .ForMember(d => d.MedicalRecord, o => o.Ignore())
    .ForMember(d => d.Patient, o => o.Ignore())
    .ForMember(d => d.Doctor, o => o.Ignore());

            CreateMap<Prescription, PrescriptionResultDto>()
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString()));
        }
    }
}
