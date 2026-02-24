using Domain.Models.DoctorModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.DoctorModule
{
    public class DoctorWithDetailsSpecification :BaseSpecifications<Doctor,int>
    {
        public DoctorWithDetailsSpecification(int id):base(d=>d.Id ==id)
        {
            AddInclude(d => d.Department);
            AddInclude(d => d.Qualifications);
            AddInclude(d => d.AvailabilitySchedules);
        }
    }
}
