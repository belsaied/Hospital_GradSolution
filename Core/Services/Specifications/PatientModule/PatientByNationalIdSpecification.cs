using Domain.Models.PatientModule;

namespace Services.Specifications.PatientModule
{
    public class PatientByNationalIdSpecification : BaseSpecifications<Patient,int>
    {
        public PatientByNationalIdSpecification(string nationalId)
    : base(p => p.NationalId == nationalId) { }
    }
}
