using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models
{
     public class EmergencyContact : BaseEntity<int>
     {
        public int PatientId { get; set; }
        public string Name { get; set; } = default!;
        public string RelationShip { get; set; } = default!;
        public string Phone { get; set; } = default!;
        public string? Email { get; set; }
        public bool IsPrimaryContactFlag { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }

        #region Navigation Property
        public Patient Patient { get; set; } = null!;

        #endregion    

     }
 }
