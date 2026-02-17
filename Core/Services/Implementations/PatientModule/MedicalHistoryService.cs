using AutoMapper;
using Domain.Contracts;
using Domain.Models.PatientModule;
using Services.Abstraction.Contracts;
using Shared.Dtos.PatientModule.Medical_History_Dtos;

namespace Services.Implementations.PatientModule
{
    public class MedicalHistoryService (IUnitOfWork _unitOfWork , IMapper _mapper) : IMedicalHistoryService
    {
        public async Task<MedicalHistoryResultDto> AddMedicalHistoryAsync(int patientId, CreateMedicalHistoryDto historyDto)
        {
            // STEP 1: Verify patient exists
            var patientRepository = _unitOfWork.GetRepository<Patient, int>();
            var patient = await patientRepository.GetByIdAsync(patientId);
            if (patient is null)
                return null!;

            // STEP 2: Map DTO to Entity
            var medicalHistory = _mapper.Map<PatientMedicalHistory>(historyDto);
            medicalHistory.PatientId = patientId;
            medicalHistory.RecordedDate = DateTime.UtcNow;
            medicalHistory.IsActive = true;

            // STEP 3: Get medical history repository and add
            var historyRepository = _unitOfWork.GetRepository<PatientMedicalHistory, int>();
            await historyRepository.AddAsync(medicalHistory);

            // STEP 4: Save changes
            await _unitOfWork.SaveChangesAsync();

            // STEP 5: Return result DTO
            return _mapper.Map<MedicalHistoryResultDto>(medicalHistory);
        }

        public async Task<IEnumerable<MedicalHistoryResultDto>> GetPatientMedicalHistoryAsync(int patientId)
        {
            // STEP 1: Verify patient exists
            var patientRepository = _unitOfWork.GetRepository<Patient, int>();
            var patient = await patientRepository.GetByIdAsync(patientId);
            if (patient is null)
                return Enumerable.Empty<MedicalHistoryResultDto>();

            // STEP 2: Get all medical histories
            var historyRepository = _unitOfWork.GetRepository<PatientMedicalHistory, int>();
            var histories = await historyRepository.GetAllAsync(asNoTracking: true);

            // STEP 3: Filter by patient ID
            var patientHistories = histories.Where(h => h.PatientId == patientId);

            // STEP 4: Map to DTOs
            return _mapper.Map<IEnumerable<MedicalHistoryResultDto>>(patientHistories);
        }

        public async Task<MedicalHistoryResultDto> UpdateMedicalHistoryAsync(int patientId, int historyId, UpdateMedicalHistoryDto historyDto)
        {
            // STEP 1: Get medical history repository
            var historyRepository = _unitOfWork.GetRepository<PatientMedicalHistory, int>();
            var medicalHistory = await historyRepository.GetByIdAsync(historyId);

            // STEP 2: Verify medical history exists and belongs to patient
            if (medicalHistory is null || medicalHistory.PatientId != patientId)
                return null!;

            // STEP 3: Update fields if provided
            if (!string.IsNullOrEmpty(historyDto.Treatment))
                medicalHistory.Treatment = historyDto.Treatment;

            if (historyDto.ResolutionDate.HasValue)
                medicalHistory.ResolutionDate = historyDto.ResolutionDate.Value;

            if (historyDto.IsActive.HasValue)
                medicalHistory.IsActive = historyDto.IsActive.Value;

            if (!string.IsNullOrEmpty(historyDto.Notes))
                medicalHistory.Notes = historyDto.Notes;

            // STEP 4: Mark as modified
            historyRepository.Update(medicalHistory);

            // STEP 5: Save changes
            await _unitOfWork.SaveChangesAsync();

            // STEP 6: Return updated DTO
            return _mapper.Map<MedicalHistoryResultDto>(medicalHistory);

        }
    }
}
