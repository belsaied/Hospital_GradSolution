using Domain.Models.WardBedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.WardBedModule
{
    public class PatientAdmissionHistorySpecification :BaseSpecifications<Admission,int>
    {
        public PatientAdmissionHistorySpecification(int patientId)
            : base(a => a.PatientId == patientId)
        {
            AddInclude(a => a.Bed);
            AddInclude(a => a.AdmittingDoctor);
            AddOrderByDescending(a => a.AdmissionDate);
        }

    }
}
