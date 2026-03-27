using Domain.Models.PatientModule;

namespace Services.Specifications.PatientModule
{
    public sealed class PatientsByIdsSpecification : BaseSpecifications<Patient,int>
    {
        public PatientsByIdsSpecification(IEnumerable<int> ids)
    : base(p => ids.Contains(p.Id))
        { }
    }
}
