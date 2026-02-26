using Domain.Models.AppointmentModule;

namespace Services.Specifications.AppointmentModule
{
    public class PatientAppointmentsSpecification : BaseSpecifications<Appointment,int>
    {
        public PatientAppointmentsSpecification(int patientId)
    : base(a => a.PatientId == patientId)
        {
            AddInclude(a => a.Doctor);
            AddOrderByDescending(a => a.AppointmentDate);
        }
    }
}
