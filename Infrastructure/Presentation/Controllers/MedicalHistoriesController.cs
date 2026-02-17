using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.PatientModule.Medical_History_Dtos;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/patients/{patientId:int}/medical-histories")]
    public class MedicalHistoriesController (IServiceManager _serviceManager) : ControllerBase
    {
        // Add medical history for a patient
        [HttpPost]
        [ProducesResponseType(typeof(MedicalHistoryResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MedicalHistoryResultDto>> AddMedicalHistory(int patientId, [FromBody] CreateMedicalHistoryDto historyDto)
        {
            var medicalHistory = await _serviceManager.MedicalHistoryService.AddMedicalHistoryAsync(patientId, historyDto);

            if (medicalHistory is null)
                return NotFound($"Patient with ID {patientId} not found.");

            return CreatedAtAction(nameof(GetPatientMedicalHistory), new { patientId }, medicalHistory);
        }

        // Get all medical histories for a patient
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MedicalHistoryResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<MedicalHistoryResultDto>>> GetPatientMedicalHistory(int patientId)
        {
            var histories = await _serviceManager.MedicalHistoryService.GetPatientMedicalHistoryAsync(patientId);
            return Ok(histories);
        }

        // Update medical history
        [HttpPut("{historyId:int}")]
        [ProducesResponseType(typeof(MedicalHistoryResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MedicalHistoryResultDto>> UpdateMedicalHistory(
            int patientId,
            int historyId,
            [FromBody] UpdateMedicalHistoryDto historyDto)
        {
            var medicalHistory = await _serviceManager.MedicalHistoryService.UpdateMedicalHistoryAsync(patientId, historyId, historyDto);

            if (medicalHistory is null)
                return NotFound($"Medical history with ID {historyId} not found for patient {patientId}.");

            return Ok(medicalHistory);
        }
    }
}
