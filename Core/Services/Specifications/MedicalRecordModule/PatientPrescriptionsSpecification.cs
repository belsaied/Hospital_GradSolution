using Domain.Models.MedicalRecordModule;

namespace Services.Specifications.MedicalRecordModule
{
    public class PatientPrescriptionsSpecification : BaseSpecifications<Prescription, int>
    {
        public PatientPrescriptionsSpecification(int patientId) : base(p => p.PatientId == patientId)
        {
            AddOrderByDescending(p => p.PrescribedAt);
        }
    }
}
