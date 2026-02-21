using AutoMapper;
using Domain.Contracts;
using Domain.Models.PatientModule;
using Services.Abstraction.Contracts;
using Services.Exceptions;
using Services.Specifications.PatientModule;
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

            //Throw exception instead of returning null     

            if (patient is null)
                    throw new NotFoundException(nameof(Patient), patientId);

            // Check if patient is active (Business Rule)
            if (patient.Status != Domain.Models.Enums.PatientStatus.Active)
                throw new BusinessRuleException("Cannot add allergy to an inactive patient.");

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
            var patientRepository = _unitOfWork.GetRepository<Patient, int>();
            var patient = await patientRepository.GetByIdAsync(patientId);

            if (patient is null)
                throw new NotFoundException(nameof(Patient), patientId);

            var allergyRepository = _unitOfWork.GetRepository<PatientAllergy, int>();
            var spec = new PatientAllergySpecification(patientId);
            var patientAllergies = await allergyRepository.GetAllAsync(spec);

            return _mapper.Map<IEnumerable<AllergyResultDto>>(patientAllergies);
        }

        public async Task<bool> RemoveAllergyAsync(int patientId, int allergyId)
        {
            // STEP 1: Get allergy repository
            var allergyRepository = _unitOfWork.GetRepository<PatientAllergy, int>();
            var allergy = await allergyRepository.GetByIdAsync(allergyId);

            // Verify allergy exists
            if (allergy is null)
                throw new NotFoundException(nameof(PatientAllergy), allergyId);

            // STEP 2: Verify allergy exists and belongs to patient
                //if (allergy is null || allergy.PatientId != patientId)
                //    return false;
            if (allergy.PatientId != patientId)
                throw new BusinessRuleException($"Allergy with ID {allergyId} does not belong to patient {patientId}.");


            // STEP 3: Delete allergy
            allergyRepository.Delete(allergy);

            // STEP 4: Save changes
            await _unitOfWork.SaveChangesAsync();

            return true;
        }
    }
}
