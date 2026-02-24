using Domain.Models.DoctorModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.DoctorModule
{
    public class DoctorByDepartmentSpecification :BaseSpecifications<Doctor,int>
    {
        public DoctorByDepartmentSpecification(int departmentId) :base(d=>d.DepartmentId ==departmentId)
        {
            AddInclude(d => d.Department);
            AddOrderBy(d => d.LastName);
        }
    }
}
