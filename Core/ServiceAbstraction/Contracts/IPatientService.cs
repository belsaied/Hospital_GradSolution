using Shared.Dtos.PatientModule;

namespace Services.Abstraction.Contracts
{
    public interface IPatientService
    {
        // Patient CRUD Operations
        Task<PatientResultDto> RegisterPatientAsync(CreatePatientDto createPatientDto);
        Task<PatientResultDto> GetPatientByIdAsync(int id);
        Task<PatientResultDto> UpdatePatientAsync(int id, UpdatePatientDto updatePatientDto);
        Task<bool> DeactivatePatientAsync(int id);
        // Task<PaginatedResult<PatientResultDto>> GetAllPatientsAsync(PatientSearchParameters parameters);
    }
}
