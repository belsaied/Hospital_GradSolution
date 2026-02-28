using Domain.Models.Enums.MedicalRecordEnums;
using Domain.Models.MedicalRecordModule;

namespace Services.Specifications.MedicalRecordModule
{
    public class ActivePrescriptionsSpecification : BaseSpecifications<Prescription, int>
    {
        public ActivePrescriptionsSpecification(int patientId)
    : base(p => p.PatientId == patientId && p.Status == PrescriptionStatus.Active)
        {
            AddOrderByDescending(p => p.PrescribedAt);
        }
    }
}
