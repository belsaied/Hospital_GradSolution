using DocumentFormat.OpenXml.Spreadsheet;
using Domain.Models.DoctorModule;
using Shared.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.DoctorModule
{
    public class DoctorListSpecification :BaseSpecifications<Doctor,int>
    {
        public DoctorListSpecification(DoctorSpecificationParameters parameters)
            : base(d =>
                (string.IsNullOrEmpty(parameters.Search) ||
                 d.FirstName.ToLower().Contains(parameters.Search.ToLower()) ||
                 d.LastName.ToLower().Contains(parameters.Search.ToLower())) &&
                (!parameters.Status.HasValue || d.Status == parameters.Status.Value) &&
                (!parameters.DepartmentId.HasValue || d.DepartmentId == parameters.DepartmentId.Value) &&
                (string.IsNullOrEmpty(parameters.Specialization) ||
                 d.Specialization.ToLower().Contains(parameters.Specialization.ToLower()))
            )

        {
            AddInclude(d => d.Department);
            AddOrderBy(d => d.LastName);
            ApplyPagination(parameters.PageSize, parameters.PageIndex);
        }
    }
}
