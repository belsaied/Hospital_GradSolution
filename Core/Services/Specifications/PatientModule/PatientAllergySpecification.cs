using Domain.Models.PatientModule;

namespace Services.Specifications.PatientModule
{
    public class PatientAllergySpecification : BaseSpecifications<PatientAllergy,int>
    {
        public PatientAllergySpecification(int patientId)
         : base(a => a.PatientId == patientId)
        {
        }
    }
}
