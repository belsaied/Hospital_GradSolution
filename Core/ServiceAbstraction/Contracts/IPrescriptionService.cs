using Shared.Dtos.MedicalRecordsDto;

namespace Services.Abstraction.Contracts
{
    public interface IPrescriptionService
    {
        Task<PrescriptionResultDto> AddPrescriptionAsync(int medicalRecordId, CreatePrescriptionDto dto);
        Task<IEnumerable<PrescriptionResultDto>> GetPatientPrescriptionsAsync(int patientId);
        Task<IEnumerable<PrescriptionResultDto>> GetActivePrescriptionsAsync(int patientId);
        Task<bool> CancelPrescriptionAsync(int medicalRecordId, int prescriptionId);
    }
}
