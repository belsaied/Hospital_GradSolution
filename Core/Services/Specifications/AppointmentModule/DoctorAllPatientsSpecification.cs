using Domain.Models.AppointmentModule;

namespace Services.Specifications.AppointmentModule
{
    public class DoctorAllPatientsSpecification : BaseSpecifications<Appointment,int>
    {
        public DoctorAllPatientsSpecification(int doctorId)
    : base(a => a.DoctorId == doctorId)
        {
            AddInclude(a => a.Patient);
            AddOrderByDescending(a => a.AppointmentDate);
        }
    }
}
