using Domain.Models.Enums.WardBedEnums;
using Domain.Models.WardBedModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.WardBedModule
{
    public class ActiveAdmissionForPatientSpecification :BaseSpecifications<Admission,int>
    {
        public ActiveAdmissionForPatientSpecification(int patientId) : base(a => a.PatientId == patientId && a.Status == AdmissionStatus.Active)
        { 

        }

    }
}
