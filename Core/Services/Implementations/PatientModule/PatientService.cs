using AutoMapper;
using Domain.Contracts;
using Domain.Models.Enums;
using Domain.Models.PatientModule;
using Services.Abstraction.Contracts;
using Shared.Dtos.PatientModule;

namespace Services.Implementations.PatientModule
{
    public class PatientService(IUnitOfWork _unitOfWork , IMapper _mapper) : IPatientService
    {
        public async Task<bool> DeactivatePatientAsync(int id)
        {
            // soft Delete by changing status to Inactive
            var patientRepository = _unitOfWork.GetRepository<Patient , int>();
            var patient = await patientRepository.GetByIdAsync(id);
            if (patient is null)
                return false;     // patient not found

            patient.Status = PatientStatus.Inactive;
            patientRepository.Update(patient);
            await _unitOfWork.SaveChangesAsync();
            return true;      // successfully deactivated
        }

        public Task<PatientResultDto> GetPatientByIdAsync(int id)
        {
            var patientRepository = _unitOfWork.GetRepository<Patient , int>();
            var patient = patientRepository.GetByIdAsync(id);
            if (patient is null)
                return null!;

            return _mapper.Map<PatientResultDto>(patient);
        }

        public async Task<PatientResultDto> RegisterPatientAsync(CreatePatientDto createPatientDto)
        {
            // STEP 1: Convert DTO to Entity
            var patient = _mapper.Map<Patient>(createPatientDto);
            // STEP 2: Generate Unique Medical Record Number
            patient.MedicalRecordNumber = await GenerateMedicalRecordNumberAsync();
            // STEP 3: set system generated fields
            patient.RegistrationDate = DateTime.UtcNow;
            patient.Status = PatientStatus.Active;
            // STEP 4: get patient repository
            var patientRepository = _unitOfWork.GetRepository<Patient , int>();
            // STEP 5: Add patient to repository
            await patientRepository.AddAsync(patient);
            // STEP 6: Commit changes
            await _unitOfWork.SaveChangesAsync();
            // STEP 7: Convert Entity back to Result DTO
            return _mapper.Map<PatientResultDto>(patient);
        }

        private async Task<string> GenerateMedicalRecordNumberAsync()
        {
            // 1] Get patient Repository
            var patientRepository = _unitOfWork.GetRepository<Patient , int>();
            // 2] Get all patients to find the max medical record number
            var allPatients = await patientRepository.GetAllAsync(asNoTracking : true);
            // 3] Count
            var count = allPatients.Count();
            // 4] Generate mrn
            var mrn = $"MRN{(count + 1):D6}";
            return mrn;
        }

        public async Task<PatientResultDto> UpdatePatientAsync(int id, UpdatePatientDto updatePatientDto)
        {
            var patientRepository = _unitOfWork.GetRepository<Patient , int>();
            var patient =await patientRepository.GetByIdAsync(id);
            if (patient is null)
                return null!;
            // Update fields

            // Update FirstName if provided
            if (!string.IsNullOrEmpty(updatePatientDto.FirstName))
                patient.FirstName = updatePatientDto.FirstName;

            // Update LastName if provided
            if (!string.IsNullOrEmpty(updatePatientDto.LastName))
                patient.LastName = updatePatientDto.LastName;

            // Update Phone if provided
            if (!string.IsNullOrEmpty(updatePatientDto.Phone))
                patient.Phone = updatePatientDto.Phone;

            // Update Email if provided
            if (!string.IsNullOrEmpty(updatePatientDto.Email))
                patient.Email = updatePatientDto.Email;

            // Update Address if provided
            // Here we map the entire Address object if it's not null
            if (updatePatientDto.Address is not null)
                patient.Address = _mapper.Map<Address>(updatePatientDto.Address);

            // Update Status if provided
            // HasValue checks if the nullable enum has a value
            if (updatePatientDto.Status.HasValue)
                patient.Status = updatePatientDto.Status.Value;

            // STEP 5: Mark the entity as modified
            // This tells Entity Framework to generate an UPDATE SQL statement
            patientRepository.Update(patient);

            // STEP 6: Save changes to database
            // This executes the UPDATE statement
            await _unitOfWork.SaveChangesAsync();

            // STEP 7: Return the updated patient as DTO
            return _mapper.Map<PatientResultDto>(patient);
        }
    }
}
