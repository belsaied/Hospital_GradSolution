using Services.Abstraction.Contracts;
using Shared.Dtos.PatientModule;

namespace Services.Implementations.PatientModule
{
    public class PatientService : IPatientService
    {
        public Task<bool> DeactivatePatientAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PatientResultDto> GetPatientByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<PatientResultDto> RegisterPatientAsync(CreatePatientDto createPatientDto)
        {
            throw new NotImplementedException();
        }

        public Task<PatientResultDto> UpdatePatientAsync(int id, UpdatePatientDto updatePatientDto)
        {
            throw new NotImplementedException();
        }
    }
}
