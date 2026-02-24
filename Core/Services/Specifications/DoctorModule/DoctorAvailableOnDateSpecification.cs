using Domain.Models.DoctorModule;
using Domain.Models.Enums.DoctorEnums;
using System;
using System.Collections.Generic;
using System.Text;
using DayOfWeek = System.DayOfWeek;

namespace Services.Specifications.DoctorModule
{
    public class DoctorAvailableOnDateSpecification:BaseSpecifications<Doctor,int>
    {
        public DoctorAvailableOnDateSpecification(DateTime date)
            : base(d => d.Status == DoctorStatus.Active &&
                        d.AvailabilitySchedules.Any(s =>
                            s.IsAvailable &&
                            s.DayOfWeek == (DayOfWeek)((int)date.DayOfWeek == 0 ? 7 : (int)date.DayOfWeek)))
        {
            AddInclude(d => d.Department);
            AddInclude(d => d.AvailabilitySchedules);
        }
    }
}
