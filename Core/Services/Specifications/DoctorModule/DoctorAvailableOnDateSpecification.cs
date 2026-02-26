using Domain.Models.DoctorModule;
using Domain.Models.Enums.DoctorEnums;
using DayOfWeek = Domain.Models.Enums.DoctorEnums.DayOfWeek;


namespace Services.Specifications.DoctorModule
{
    public class DoctorAvailableOnDateSpecification:BaseSpecifications<Doctor,int>
    {
        public DoctorAvailableOnDateSpecification(DateTime date)
            : base(BuildCriteria(date))
        {
            AddInclude(d => d.Department);
            AddInclude(d => d.AvailabilitySchedules);
        }

        private static System.Linq.Expressions.Expression<Func<Doctor, bool>> BuildCriteria(DateTime date)
        {
            var customDay = (DayOfWeek)((int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek);
            return d => d.Status == DoctorStatus.Active &&
                        d.AvailabilitySchedules.Any(s =>
                            s.IsAvailable &&
                            s.DayOfWeek == customDay);
        }
    }
}
