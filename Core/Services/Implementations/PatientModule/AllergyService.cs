using AutoMapper;
using Domain.Contracts;
using Domain.Models.PatientModule;
using Services.Abstraction.Contracts;
using Shared.Dtos.PatientModule.AllergyDtos;

namespace Services.Implementations.PatientModule
{
    public class AllergyService(IUnitOfWork _unitOfWork, IMapper _mapper) : IAllergyService
    {
        public async Task<AllergyResultDto> AddAllergyAsync(int patientId, CreateAllergyDto allergyDto)
        {
            // STEP 1: Verify patient exists
            var patientRepository = _unitOfWork.GetRepository<Patient, int>();
            var patient = await patientRepository.GetByIdAsync(patientId);
            if (patient is null)
                return null!;

            // STEP 2: Map DTO to Entity
            var allergy = _mapper.Map<PatientAllergy>(allergyDto);
            allergy.PatientId = patientId;
            allergy.RecordDate = DateTime.UtcNow;

            // STEP 3: Get allergy repository and add
            var allergyRepository = _unitOfWork.GetRepository<PatientAllergy, int>();
            await allergyRepository.AddAsync(allergy);

            // STEP 4: Save changes
            await _unitOfWork.SaveChangesAsync();

            // STEP 5: Return result DTO
            return _mapper.Map<AllergyResultDto>(allergy);
        }

        public async Task<IEnumerable<AllergyResultDto>> GetPatientAllergiesAsync(int patientId)
        {
            // STEP 1: Verify patient exists
            var patientRepository = _unitOfWork.GetRepository<Patient, int>();
            var patient = await patientRepository.GetByIdAsync(patientId);
            if (patient is null)
                return Enumerable.Empty<AllergyResultDto>();

            // STEP 2: Get all allergies
            var allergyRepository = _unitOfWork.GetRepository<PatientAllergy, int>();
            var allergies = await allergyRepository.GetAllAsync(asNoTracking: true);

            // STEP 3: Filter by patient ID
            var patientAllergies = allergies.Where(a => a.PatientId == patientId);

            // STEP 4: Map to DTOs
            return _mapper.Map<IEnumerable<AllergyResultDto>>(patientAllergies);
        }

        public async Task<bool> RemoveAllergyAsync(int patientId, int allergyId)
        {
            // STEP 1: Get allergy repository
            var allergyRepository = _unitOfWork.GetRepository<PatientAllergy, int>();
            var allergy = await allergyRepository.GetByIdAsync(allergyId);

            // STEP 2: Verify allergy exists and belongs to patient
            if (allergy is null || allergy.PatientId != patientId)
                return false;

            // STEP 3: Delete allergy
            allergyRepository.Delete(allergy);

            // STEP 4: Save changes
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
