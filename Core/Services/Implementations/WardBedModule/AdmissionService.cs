using AutoMapper;
using Domain.Contracts;
using Domain.Models.DoctorModule;
using Domain.Models.Enums.DoctorEnums;
using Domain.Models.Enums.PatientEnums;
using Domain.Models.Enums.WardBedEnums;
using Domain.Models.PatientModule;
using Domain.Models.WardBedModule;
using Services.Abstraction.Contracts.WardBedService;
using Services.Exceptions;
using Services.Specifications.WardBedModule;
using Shared.Dtos.WardBedModule.AdmissionDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Implementations.WardBedModule
{
    public class AdmissionService( IUnitOfWork _unitOfWork,  IMapper _mapper, IBedNotifier _notifier) : IAdmissionService
    {
        public async Task<AdmissionResultDto> AdmitPatientAsync(CreateAdmissionDto dto)
        {
            // 1. Validate patient exists
            var patientRepo = _unitOfWork.GetRepository<Patient, int>();
            var patient = await patientRepo.GetByIdAsync(dto.PatientId);
            if (patient is null) throw new PatientNotFoundException(dto.PatientId);

            //  BR FIX: Patient must be Active (not Inactive or Deceased)
            if (patient.Status != PatientStatus.Active)
                throw new BusinessRuleException(
                    $"Cannot admit patient with status '{patient.Status}'. Only Active patients can be admitted.");

            // 2. Validate doctor exists and is Active
            var doctorRepo = _unitOfWork.GetRepository<Doctor, int>();
            var doctor = await doctorRepo.GetByIdAsync(dto.AdmittingDoctorId);
            if (doctor is null) throw new DoctorNotFoundException(dto.AdmittingDoctorId);

            if (doctor.Status != DoctorStatus.Active)
                throw new BusinessRuleException(
                    $"Cannot assign doctor with status '{doctor.Status}'. Only Active doctors can admit patients.");

            // 3. Validate bed exists and is Available
            var bedRepo = _unitOfWork.GetRepository<Bed, int>();
            var bed = await bedRepo.GetByIdAsync(dto.BedId);
            if (bed is null) throw new BedNotFoundException(dto.BedId);
            if (bed.Status != BedStatus.Available)
                throw new BusinessRuleException(
                    $"Bed {dto.BedId} is not available. Current status: {bed.Status}.");

            // 4. BR: Patient cannot have another active admission
            var admissionRepo = _unitOfWork.GetRepository<Admission, int>();
            var existing = await admissionRepo.GetAllAsync(
                new ActiveAdmissionForPatientSpecification(dto.PatientId));
            if (existing.Any())
                throw new BusinessRuleException(
                    "Patient already has an active admission. Discharge first before re-admitting.");

            // 5. Create admission
            var admission = _mapper.Map<Admission>(dto);

            // FIX: Use DTO AdmissionDate if provided, otherwise fallback to UtcNow
            admission.AdmissionDate = dto.AdmissionDate == default
                ? DateTime.UtcNow
                : dto.AdmissionDate;

            admission.Status = AdmissionStatus.Active;

            await admissionRepo.AddAsync(admission);

            // 6. Mark bed as Occupied
            bed.Status = BedStatus.Occupied;
            bedRepo.Update(bed);

            await _unitOfWork.SaveChangesAsync();

            // 7. SignalR: BRD event name is "BedOccupied"
            await _notifier.NotifyDashboardAsync("BedOccupied", new
            {
                bedId = dto.BedId,
                bedNumber = bed.BedNumber,
                roomId = bed.RoomId,
                patientId = dto.PatientId,
                admissionId = admission.Id
            });

            // 8. Reload with navigation
            var saved = await admissionRepo.GetByIdAsync(
                new AdmissionWithDetailsSpecification(admission.Id));
            return _mapper.Map<AdmissionResultDto>(saved!);
        }

        public async Task<AdmissionResultDto> GetAdmissionByIdAsync(int admissionId)
        {
            var repo = _unitOfWork.GetRepository<Admission, int>();
            var admission = await repo.GetByIdAsync(
                new AdmissionWithDetailsSpecification(admissionId));
            if (admission is null) throw new AdmissionNotFoundException(admissionId);
            return _mapper.Map<AdmissionResultDto>(admission);
        }

        public async Task<IEnumerable<AdmissionResultDto>> GetPatientAdmissionHistoryAsync(int patientId)
        {
            var patientRepo = _unitOfWork.GetRepository<Patient, int>();
            if (await patientRepo.GetByIdAsync(patientId) is null)
                throw new PatientNotFoundException(patientId);

            var repo = _unitOfWork.GetRepository<Admission, int>();
            var admissions = await repo.GetAllAsync(
                new PatientAdmissionHistorySpecification(patientId));
            return _mapper.Map<IEnumerable<AdmissionResultDto>>(admissions);
        }

        public async Task<IEnumerable<AdmissionResultDto>> GetActiveAdmissionsAsync()
        {
            var repo = _unitOfWork.GetRepository<Admission, int>();
            var admissions = await repo.GetAllAsync(new ActiveAdmissionsSpecification());
            return _mapper.Map<IEnumerable<AdmissionResultDto>>(admissions);
        }

        public async Task<AdmissionResultDto> DischargePatientAsync(int admissionId, DischargeDto dto)
        {
            var admissionRepo = _unitOfWork.GetRepository<Admission, int>();
            var admission = await admissionRepo.GetByIdAsync(
                new AdmissionWithDetailsSpecification(admissionId));
            if (admission is null) throw new AdmissionNotFoundException(admissionId);

            if (admission.Status != AdmissionStatus.Active)
                throw new BusinessRuleException(
                    $"Cannot discharge. Admission status is: {admission.Status}.");

            // Update admission
            admission.Status = AdmissionStatus.Discharged;
            admission.ActualDischargeDate = DateTime.UtcNow;
            admission.DischargeSummary = dto.DischargeSummary;
            admissionRepo.Update(admission);

            // Free the bed
            var bedRepo = _unitOfWork.GetRepository<Bed, int>();
            var bed = await bedRepo.GetByIdAsync(admission.BedId);
            if (bed is not null)
            {
                bed.Status = BedStatus.Available;
                bedRepo.Update(bed);
            }

            await _unitOfWork.SaveChangesAsync();

            // SignalR: BRD event : BedReleased
            await _notifier.NotifyDashboardAsync("BedReleased", new
            {
                bedId = admission.BedId,
                bedNumber = bed?.BedNumber,
                roomId = bed?.RoomId,
                newStatus = BedStatus.Available.ToString()
            });

            // TODO (Phase 2 Billing hook): emit discharge event to trigger invoice creation

            var updated = await admissionRepo.GetByIdAsync(
                new AdmissionWithDetailsSpecification(admissionId));
            return _mapper.Map<AdmissionResultDto>(updated!);
        }

        public async Task<AdmissionResultDto> TransferPatientAsync(int admissionId, TransferBedDto dto)
        {
            var admissionRepo = _unitOfWork.GetRepository<Admission, int>();
            var admission = await admissionRepo.GetByIdAsync(
                new AdmissionWithDetailsSpecification(admissionId));
            if (admission is null) throw new AdmissionNotFoundException(admissionId);

            if (admission.Status != AdmissionStatus.Active)
                throw new BusinessRuleException("Can only transfer an Active admission.");

            // BR FIX: ToBedId must be different from current bed
            if (dto.ToBedId == admission.BedId)
                throw new BusinessRuleException(
                    "Target bed must be different from the patient's current bed.");

            // Validate new bed
            var bedRepo = _unitOfWork.GetRepository<Bed, int>();
            var newBed = await bedRepo.GetByIdAsync(dto.ToBedId);
            if (newBed is null) throw new BedNotFoundException(dto.ToBedId);
            if (newBed.Status != BedStatus.Available)
                throw new BusinessRuleException(
                    $"Target bed {dto.ToBedId} is not available. Status: {newBed.Status}.");

            int fromBedId = admission.BedId;

            // Create transfer record
            var transfer = new BedTransfer
            {
                AdmissionId = admissionId,
                FromBedId = fromBedId,
                ToBedId = dto.ToBedId,
                TransferredAt = DateTime.UtcNow,
                Reason = dto.Reason,
                TransferredBy = dto.TransferredBy
            };

            var transferRepo = _unitOfWork.GetRepository<BedTransfer, int>();
            await transferRepo.AddAsync(transfer);

            // Update admission bed
            admission.BedId = dto.ToBedId;
            admissionRepo.Update(admission);

            // Free old bed, occupy new bed
            var oldBed = await bedRepo.GetByIdAsync(fromBedId);
            if (oldBed is not null) { oldBed.Status = BedStatus.Available; bedRepo.Update(oldBed); }
            newBed.Status = BedStatus.Occupied;
            bedRepo.Update(newBed);

            await _unitOfWork.SaveChangesAsync();

            // SignalR: BRD event :BedTransferred
            await _notifier.NotifyDashboardAsync("BedTransferred", new
            {
                fromBedId,
                toBedId = dto.ToBedId,
                admissionId,
                reason = dto.Reason
            });

            var updated = await admissionRepo.GetByIdAsync(
                new AdmissionWithDetailsSpecification(admissionId));
            return _mapper.Map<AdmissionResultDto>(updated!);
        }
    }
}
