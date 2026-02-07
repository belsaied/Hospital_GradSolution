using Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class Patient:BaseEntity<int>
    {  
        
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public Gender Gender { get; set; } = default!;
        public BloodType? BloodType { get; set; }
        public string NationalId { get; set; } = default!;
        public string MedicalRecordNumber { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string Email { get; set; } = default!;
        public Address Address { get; set; } = null!;
        public PatientStatus Status { get; set; } = default!;
        public DateTime RegistrationDate { get; set; }

        #region Navigation Property
        public ICollection<PatientAllergy> PatientAllergies { get; set; } = new HashSet<PatientAllergy>();
        public ICollection<PatientMedicalHistory> MedicalHistories { get; set; } = new HashSet<PatientMedicalHistory>();
        public ICollection<EmergencyContact> EmergencyContacts { get; set; } = new HashSet<EmergencyContact>();
        #endregion 
    }
}
