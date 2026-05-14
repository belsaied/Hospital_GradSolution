using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.MedicalRecordsDto;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/patients/{patientId:int}/prescriptions")]
    [Authorize]
    public class PatientPrescriptionsController (IServiceManager _serviceManager) : ControllerBase
    {
        // GET /api/patients/{patientId}/prescriptions
        [Authorize(Roles = "SuperAdmin,Doctor,Nurse,Patient")]
        [Authorize(Policy = "PatientOwnership")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PrescriptionResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PrescriptionResultDto>>> GetPatientPrescriptions(
    int patientId)
    => Ok(await _serviceManager.PrescriptionService.GetPatientPrescriptionsAsync(patientId));

        // GET /api/patients/{patientId}/prescriptions/active
        [Authorize(Roles = "SuperAdmin,Doctor,Nurse,Patient")]
        [Authorize(Policy = "PatientOwnership")]
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<PrescriptionResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<PrescriptionResultDto>>> GetActivePrescriptions(
            int patientId)
            => Ok(await _serviceManager.PrescriptionService.GetActivePrescriptionsAsync(patientId));

        // GET /api/patients/{patientId}/prescriptions/{prescriptionId}/pdf   
        [Authorize(Roles = "SuperAdmin,Doctor,Nurse,Patient")]
        [Authorize(Policy = "PatientOwnership")]
        [HttpGet("{prescriptionId:int}/pdf")]
        [ProducesResponseType(typeof(FileResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DownloadPrescriptionPdf(int patientId, int prescriptionId)
        {
            var bytes = await _serviceManager.PrescriptionService
                .GeneratePrescriptionPdfAsync(prescriptionId, patientId);

            return File(bytes, "application/pdf",
                $"prescription-RX-{prescriptionId:D8}.pdf");
        }
    }
}
