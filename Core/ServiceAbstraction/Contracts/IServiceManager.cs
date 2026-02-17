namespace Services.Abstraction.Contracts
{
    public interface IServiceManager
    {
        public IPatientService PatientService { get; }
        public IAllergyService AllergyService { get; }
        public IMedicalHistoryService MedicalHistoryService { get; }
        public IEmergencyContactService EmergencyContactService { get; }
    }
}
