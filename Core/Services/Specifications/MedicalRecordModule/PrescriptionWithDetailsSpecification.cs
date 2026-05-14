using Domain.Models.MedicalRecordModule;

namespace Services.Specifications.MedicalRecordModule
{
    public class PrescriptionWithDetailsSpecification : BaseSpecifications<Prescription,int>
    {
        public PrescriptionWithDetailsSpecification(int id) : base(p => p.Id == id)
        {
            AddInclude(p => p.MedicalRecord);
            AddInclude(p => p.Patient);
            AddInclude(p => p.Doctor);
        }
    }
}
