using Services.Abstraction.Contracts;

namespace Services.Implementations
{
    public class ServiceManagerWithFactoryDelegate(Func<IPatientService> _patientService
        , Func<IAllergyService> _allergyService , Func<IEmergencyContactService> _emergencyContactService,
        Func<IMedicalHistoryService> _medicalHistoryService) : IServiceManager
    {
        public IPatientService PatientService => _patientService.Invoke();

        public IAllergyService AllergyService => _allergyService.Invoke();

        public IEmergencyContactService EmergencyContactService => _emergencyContactService.Invoke();

        public IMedicalHistoryService MedicalHistoryService => _medicalHistoryService.Invoke();
    }
}
