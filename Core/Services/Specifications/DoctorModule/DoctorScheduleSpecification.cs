using Domain.Models.DoctorModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Specifications.DoctorModule
{
    public class DoctorScheduleSpecification :BaseSpecifications<DoctorSchedule,int>
    {
        //// لجيب كل جداول دكتور معين
        public DoctorScheduleSpecification(int doctorId):base(s=>s.DoctorId== doctorId)
        {
            
        }
    }
}
