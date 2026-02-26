using Domain.Models.AppointmentModule;

namespace Services.Specifications.AppointmentModule
{
    public class ConfirmationNumberExistsSpecification : BaseSpecifications<Appointment,int>
    {
        public ConfirmationNumberExistsSpecification(string number)
              : base(a => a.ConfirmationNumber == number) { }
    }
}
