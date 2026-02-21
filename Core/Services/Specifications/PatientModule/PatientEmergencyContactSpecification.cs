using Domain.Models.PatientModule;

namespace Services.Specifications.PatientModule
{
    public class PatientEmergencyContactSpecification : BaseSpecifications<EmergencyContact,int>
    {
        public PatientEmergencyContactSpecification(int patientId)
    : base(c => c.PatientId == patientId)
        {
        }
    }
}
