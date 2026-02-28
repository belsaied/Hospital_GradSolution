using Domain.Models.MedicalRecordModule;

namespace Services.Specifications.MedicalRecordModule
{
    public class DoctorMedicalRecordsSpecification : BaseSpecifications<MedicalRecord, int>
    {
        public DoctorMedicalRecordsSpecification(int doctorId) : base(r => r.DoctorId == doctorId)
        {
            AddInclude(r => r.Patient);
            AddOrderByDescending(r => r.VisitDate);
        }
    }
}
