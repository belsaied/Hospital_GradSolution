using Shared.Dtos.MedicalRecordsDto;

namespace Services.Abstraction.Contracts
{
    public interface ILabOrderService
    {
        Task<LabOrderResultDto> CreateLabOrderAsync(int medicalRecordId, CreateLabOrderDto dto);
        Task<IEnumerable<LabOrderResultDto>> GetPatientLabOrdersAsync(int patientId);
        Task<LabOrderResultDto> UpdateLabOrderStatusAsync(int orderId, UpdateLabOrderStatusDto dto);
        Task<LabResultResultDto> AddLabResultAsync(int orderId, CreateLabResultDto dto);
    }
}
