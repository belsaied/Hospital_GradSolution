using Domain.Models.PatientModule;

namespace Services.Specifications.PatientModule
{
    public class PatientByEmailSpecification : BaseSpecifications<Patient,int>
    {
        public PatientByEmailSpecification(string email)
    : base(p => p.Email.ToLower() == email.ToLower()) { }
    }
}
