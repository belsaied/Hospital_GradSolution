using Shared;
using Shared.Dtos.MedicalRecordsDto;
using Shared.Parameters;

namespace Services.Abstraction.Contracts
{
    public interface IMedicalRecordService
    {
        Task<MedicalRecordResultDto> CreateMedicalRecordAsync(CreateMedicalRecordDto dto);
        Task<MedicalRecordResultDto> GetMedicalRecordByIdAsync(int id);
        Task<MedicalRecordResultDto> UpdateMedicalRecordAsync(int id, UpdateMedicalRecordDto dto, int requestingDoctorId);
        Task<PaginatedResult<MedicalRecordResultDto>> GetPatientMedicalRecordsAsync(int patientId, MedicalRecordSpecificationParameters p);
        Task<PaginatedResult<MedicalRecordResultDto>> GetDoctorMedicalRecordsAsync(int doctorId, MedicalRecordSpecificationParameters p);
    }
}
