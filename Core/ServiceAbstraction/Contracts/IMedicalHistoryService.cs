using Shared.Dtos.PatientModule.Medical_History_Dtos;

namespace Services.Abstraction.Contracts
{
    public interface IMedicalHistoryService
    {
        Task<MedicalHistoryResultDto> AddMedicalHistoryAsync(int patientId, CreateMedicalHistoryDto historyDto);
        Task<IEnumerable<MedicalHistoryResultDto>> GetPatientMedicalHistoryAsync(int patientId);
        Task<MedicalHistoryResultDto> UpdateMedicalHistoryAsync(int patientId, int historyId, UpdateMedicalHistoryDto historyDto);
    }
}
