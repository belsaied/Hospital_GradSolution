namespace Services.Abstraction.Contracts
{
    public interface IServiceManager
    {
        #region Patient
        public IPatientService PatientService { get; }
        public IAllergyService AllergyService { get; }
        public IMedicalHistoryService MedicalHistoryService { get; }
        public IEmergencyContactService EmergencyContactService { get; }
        #endregion

        #region Doctor
        public IDoctorService DoctorService { get; }
        public IDepartmentService DepartmentService { get; }

        #endregion
    }
}
