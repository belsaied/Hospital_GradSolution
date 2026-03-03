using Domain.Models.WardBedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.WardBedModule
{
    public class AdmissionWithDetailsSpecification :BaseSpecifications<Admission,int>
    {
        public AdmissionWithDetailsSpecification(int id) : base(a => a.Id == id)
        {
            AddInclude(a => a.Patient);
            AddInclude(a => a.Bed);
            AddInclude(a => a.AdmittingDoctor);
        }

    }
}
