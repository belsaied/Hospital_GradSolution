using AutoMapper;
using Domain.Contracts;
using Domain.Models.Enums.MedicalRecordEnums;
using Domain.Models.MedicalRecordModule;
using Domain.Models.PatientModule;
using Services.Abstraction.Contracts;
using Services.Exceptions;
using Services.Specifications.MedicalRecordModule;
using Shared.Dtos.MedicalRecordsDto;

namespace Services.Implementations.MedicalRecordModule
{
    public class LabOrderService (IUnitOfWork _unitOfWork, IMapper _mapper) : ILabOrderService
    {
        public async Task<LabOrderResultDto> CreateLabOrderAsync(int medicalRecordId, CreateLabOrderDto dto)
        {
            // 1. Validate record
            var recordRepo = _unitOfWork.GetRepository<MedicalRecord, int>();
            var record = await recordRepo.GetByIdAsync(medicalRecordId);
            if (record is null) throw new MedicalRecordNotFoundException(medicalRecordId);

            // 2. Stat orders require non-empty Notes
            if (dto.Priority == LabOrderPriority.Stat && string.IsNullOrWhiteSpace(dto.Notes))
                throw new ValidationException("Stat priority lab orders require Notes explaining clinical urgency.");

            // 3. Map and save
            var labOrder = _mapper.Map<LabOrder>(dto);
            labOrder.MedicalRecordId = medicalRecordId;
            labOrder.PatientId = record.PatientId;
            labOrder.DoctorId = record.DoctorId;
            labOrder.OrderedAt = DateTime.UtcNow;
            labOrder.Status = LabOrderStatus.Pending;

            var orderRepo = _unitOfWork.GetRepository<LabOrder, int>();
            await orderRepo.AddAsync(labOrder);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<LabOrderResultDto>(labOrder);
        }

        public async Task<IEnumerable<LabOrderResultDto>> GetPatientLabOrdersAsync(int patientId)
        {
            var patientRepo = _unitOfWork.GetRepository<Patient, int>();
            if (await patientRepo.GetByIdAsync(patientId) is null)
                throw new PatientNotFoundException(patientId);

            var orderRepo = _unitOfWork.GetRepository<LabOrder, int>();
            var orders = await orderRepo.GetAllAsync(new PatientLabOrdersSpecification(patientId));
            return _mapper.Map<IEnumerable<LabOrderResultDto>>(orders);
        }

        public async Task<LabOrderResultDto> UpdateLabOrderStatusAsync(int orderId, UpdateLabOrderStatusDto dto)
        {
            var orderRepo = _unitOfWork.GetRepository<LabOrder, int>();
            var order = await orderRepo.GetByIdAsync(orderId);
            if (order is null) throw new NotFoundException("LabOrder", orderId);

            // Guard against invalid transitions — result attachment handles Completed separately
            if (order.Status == LabOrderStatus.Completed || order.Status == LabOrderStatus.Cacelled)
                throw new BusinessRuleException(
                    $"Cannot change status of a {order.Status} lab order.");

            order.Status = dto.NewStatus;
            orderRepo.Update(order);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<LabOrderResultDto>(order);
        }

        public async Task<LabResultResultDto> AddLabResultAsync(int orderId, CreateLabResultDto dto)
        {
            var orderRepo = _unitOfWork.GetRepository<LabOrder, int>();
            var order = await orderRepo.GetByIdAsync(orderId);
            if (order is null) throw new NotFoundException("LabOrder", orderId);

            // Lab results can only be added to Pending or InProgress orders
            if (order.Status != LabOrderStatus.Pending && order.Status != LabOrderStatus.InProgress)
                throw new BusinessRuleException(
                    $"A lab result can only be added to a Pending or InProgress order. Current status: {order.Status}.");

            var result = _mapper.Map<LabResult>(dto);
            result.LabOrderId = orderId;

            var resultRepo = _unitOfWork.GetRepository<LabResult, int>();
            await resultRepo.AddAsync(result);

            // Auto-transition order to Completed
            order.Status = LabOrderStatus.Completed;
            orderRepo.Update(order);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<LabResultResultDto>(result);
        }
    }
}
