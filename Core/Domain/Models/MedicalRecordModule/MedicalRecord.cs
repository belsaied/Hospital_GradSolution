using Domain.Models.AppointmentModule;
using Domain.Models.DoctorModule;
using Domain.Models.PatientModule;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.MedicalRecordModule
{
    public class MedicalRecord :BaseEntity<int>
    {
        //FK
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public int? AppointmentId { get; set; }

        //Clinical Data
        public DateTime VisiteDate { get; set; }
        public string ChiefComplaint { get; set; } = string.Empty;
        public string Diagnsis { get; set; }   =string.Empty;
        public string? IcdCode { get; set; }
        public string? ClinicalNotes { get; set; }
        public string? TreatmentPlan { get; set; }
        public DateOnly? FollowUpDate { get; set; }

        //Audit
        public DateTime RecordedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        //Security
        public bool IsConfidential { get; set; } = false;


        #region Navigation Property

        public Patient Patient { get; set; } = null!;
        public Doctor Doctor { get; set; } = null!;
        public Appointment? Appointment { get; set; }

        public ICollection<VitalSign> VitalSigns { get; set; } = new List<VitalSign>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
        #endregion

    }
}
