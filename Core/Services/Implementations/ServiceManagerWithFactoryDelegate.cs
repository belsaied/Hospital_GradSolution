using Services.Abstraction.Contracts;

namespace Services.Implementations
{
    public class ServiceManagerWithFactoryDelegate(Func<IPatientService> _patientService) : IServiceManager
    {
        public IPatientService PatientService => _patientService.Invoke();
    }
}
