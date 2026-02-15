using Shared.Dtos.PatientModule.AllergyDtos;

namespace Services.Abstraction.Contracts
{
    public interface IAllergyService
    {
        Task<AllergyResultDto> AddAllergyAsync(int patientId, CreateAllergyDto allergyDto);
        Task<IEnumerable<AllergyResultDto>> GetPatientAllergiesAsync(int patientId);
        Task<bool> RemoveAllergyAsync(int patientId, int allergyId);
    }
}
