using Domain.Models.DoctorModule;

namespace Services.Specifications.DoctorModule
{
    public class DoctorByLicenseSpecification : BaseSpecifications<Doctor, int>
    {
        public DoctorByLicenseSpecification(string licenseNumber)
    : base(d => d.LicenseNumber == licenseNumber) { }
    }
}
