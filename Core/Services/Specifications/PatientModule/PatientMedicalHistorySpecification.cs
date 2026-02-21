using Domain.Models.PatientModule;

namespace Services.Specifications.PatientModule
{
    public class PatientMedicalHistorySpecification : BaseSpecifications<PatientMedicalHistory, int>
    {
        public PatientMedicalHistorySpecification(int patientId)
    : base(h => h.PatientId == patientId)
        {
        }
    }
}
