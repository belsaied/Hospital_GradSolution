using Shared.Dtos.MedicalRecordsDto;

namespace Services.Abstraction.Contracts
{
    public interface IVitalSignService
    {
        Task<VitalSignResultDto> AddVitalSignAsync(int medicalRecordId, CreateVitalSignDto dto);
        Task<IEnumerable<VitalSignResultDto>> GetPatientVitalHistoryAsync(int patientId);
        Task<VitalSignResultDto?> GetLatestVitalsAsync(int patientId);
    }
}
