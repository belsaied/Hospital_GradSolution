using Domain.Models.MedicalRecordModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.MedicalRecordModule
{
    public class DoctorMedicalRecordsCountSpecification :BaseSpecifications<MedicalRecord,int>
    {
        public DoctorMedicalRecordsCountSpecification(int doctorId) 
            : base(r=>r.DoctorId ==doctorId)
        {
            
        }
    }
}
