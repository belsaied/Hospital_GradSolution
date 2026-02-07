using Domain.Models.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
    public class PatientAllergy :BaseEntity<int>
    {
        public int PatientId { get; set; }
        public AllergyType Type { get; set; }
        public Severity Severity { get; set; }
        public string Description { get; set; } = default!;
        public DateTime RecordDate { get; set; }=DateTime.Now;
        public string RecordedBy { get; set; } = default!;

        #region Navigation Property
        public Patient Patient { get; set; } = null!;
        #endregion
    }
}
