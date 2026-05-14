using Domain.Models.MedicalRecordModule;

namespace Services.Abstraction.Contracts
{
    public interface IPrescriptionPdfGenerator
    {
        byte[] Generate(Prescription prescription, string patientName, string doctorName, string doctorSpecialization);

    }
}
