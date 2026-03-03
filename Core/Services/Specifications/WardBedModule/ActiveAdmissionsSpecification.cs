using Domain.Models.Enums.WardBedEnums;
using Domain.Models.WardBedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.WardBedModule
{
    public class ActiveAdmissionsSpecification :BaseSpecifications<Admission,int>
    {
        public ActiveAdmissionsSpecification() : base(a => a.Status == AdmissionStatus.Active)
        {
            AddInclude(a => a.Patient);
            AddInclude(a => a.Bed);
            AddInclude(a => a.AdmittingDoctor);
            AddOrderByDescending(a => a.AdmissionDate);
        }

    }
}
