using Shared.Dtos.MedicalRecordsDto;

namespace Services.Abstraction.Contracts
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecordResultDto> CreateMedicalRecordAsync(CreateMedicalRecordDto dto);
        Task<MedicalRecordResultDto> GetMedicalRecordByIdAsync(int id);
        Task<MedicalRecordResultDto> UpdateMedicalRecordAsync(int id, UpdateMedicalRecordDto dto, int requestingDoctorId);
        Task<IEnumerable<MedicalRecordResultDto>> GetPatientMedicalRecordsAsync(int patientId);
        Task<IEnumerable<MedicalRecordResultDto>> GetDoctorMedicalRecordsAsync(int doctorId);
    }
}
