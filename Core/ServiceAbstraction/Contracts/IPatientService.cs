using Shared;
using Shared.Dtos.PatientModule.PatientDtos;
using Shared.Parameters;

namespace Services.Abstraction.Contracts
{
    public interface IPatientService
    {
        Task<PatientResultDto> RegisterPatientAsync(CreatePatientDto createPatientDto);
        Task<PatientResultDto> GetPatientByIdAsync(int id);
        Task<PatientResultDto> UpdatePatientAsync(int id, UpdatePatientDto updatePatientDto);
        Task<bool> DeactivatePatientAsync(int id);

        // Two new endpoints
        Task<PatientWithDetailsResultDto> GetPatientWithDetailsAsync(int id);
        Task<PaginatedResult<PatientResultDto>> GetAllPatientsAsync(PatientSpecificationParameters parameters);
    }
}
