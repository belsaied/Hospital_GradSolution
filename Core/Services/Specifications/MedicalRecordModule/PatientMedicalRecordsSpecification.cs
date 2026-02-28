using Domain.Models.MedicalRecordModule;

namespace Services.Specifications.MedicalRecordModule
{
    public class PatientMedicalRecordsSpecification : BaseSpecifications<MedicalRecord, int>
    {
        public PatientMedicalRecordsSpecification(int patientId) : base(r => r.PatientId == patientId)
        {
            AddInclude(r => r.Doctor);
            AddInclude(r => r.VitalSigns);
            AddInclude(r => r.Prescriptions);
            AddInclude(r => r.LabOrders);
            AddOrderByDescending(r => r.VisitDate);
        }
    }
}
