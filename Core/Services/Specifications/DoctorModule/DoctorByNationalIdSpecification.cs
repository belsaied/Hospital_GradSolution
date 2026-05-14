using Domain.Models.DoctorModule;

namespace Services.Specifications.DoctorModule
{
    public class DoctorByNationalIdSpecification : BaseSpecifications<Doctor,int>
    {
        public DoctorByNationalIdSpecification(string nationalId)
            : base(d => d.NationalId == nationalId)
        {
            
        }
    }
}
