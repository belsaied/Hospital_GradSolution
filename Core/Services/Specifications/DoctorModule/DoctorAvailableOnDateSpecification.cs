using Domain.Models.DoctorModule;
using Domain.Models.Enums.DoctorEnums;


namespace Services.Specifications.DoctorModule
{
    public class DoctorAvailableOnDateSpecification:BaseSpecifications<Doctor,int>
    {
        public DoctorAvailableOnDateSpecification(DateTime date)
            : base(d => d.Status == DoctorStatus.Active &&
            d.AvailabilitySchedules.Any(s =>
                s.IsAvailable &&
                s.DayOfWeek == (Domain.Models.Enums.DoctorEnums.DayOfWeek)
                    (date.DayOfWeek == 0 ? 7 : date.DayOfWeek)))
        {
            AddInclude(d => d.Department);
            AddInclude(d => d.AvailabilitySchedules);
        }
    }
}
