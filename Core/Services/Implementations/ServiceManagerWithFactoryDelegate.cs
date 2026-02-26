using Services.Abstraction.Contracts;

namespace Services.Implementations
{
    public class ServiceManagerWithFactoryDelegate(
        //Patient Module
        Func<IPatientService> _patientService  ,
        Func<IAllergyService> _allergyService  ,
        Func<IEmergencyContactService> _emergencyContactService,
        Func<IMedicalHistoryService> _medicalHistoryService,
        //Doctor Module
        Func<IDoctorService> _doctorService,
        Func<IDepartmentService> _departmentService,
        Func<IAppointmentService> _appointmentService

        ) : IServiceManager
    {
        //Patient Module
        public IPatientService PatientService => _patientService.Invoke();

        public IAllergyService AllergyService => _allergyService.Invoke();

        public IEmergencyContactService EmergencyContactService => _emergencyContactService.Invoke();

        public IMedicalHistoryService MedicalHistoryService => _medicalHistoryService.Invoke();
        
        //Doctor Module
        public IDoctorService DoctorService => _doctorService.Invoke();

        public IDepartmentService DepartmentService => _departmentService.Invoke();

        public IAppointmentService AppointmentService => _appointmentService.Invoke();
    }
}
