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
        Func<IAppointmentService> _appointmentService,
                // Medical Records Module
        Func<IMedicalRecordService> _medicalRecordService,
        Func<IVitalSignService> _vitalSignService,
        Func<IPrescriptionService> _prescriptionService,
        Func<ILabOrderService> _labOrderService
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

        // Medical Records Module
        public IMedicalRecordService MedicalRecordService => _medicalRecordService.Invoke();
        public IVitalSignService VitalSignService => _vitalSignService.Invoke();
        public IPrescriptionService PrescriptionService => _prescriptionService.Invoke();
        public ILabOrderService LabOrderService => _labOrderService.Invoke();
    }
}
