using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Models.Enums.MedicalRecordEnums
{
    public enum PrescriptionStatus
    {
        Active=1,
        Completed = 2,
        Cancelled =3,
        Expired =4
    }
}
