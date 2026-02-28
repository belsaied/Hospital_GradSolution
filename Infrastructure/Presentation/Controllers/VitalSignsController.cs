using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.MedicalRecordsDto;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/patients/{patientId:int}/vitals")]
    public class VitalSignsController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<VitalSignResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<VitalSignResultDto>>> GetPatientVitals(int patientId)
    => Ok(await _serviceManager.VitalSignService.GetPatientVitalHistoryAsync(patientId));

        [HttpGet("latest")]
        [ProducesResponseType(typeof(VitalSignResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VitalSignResultDto>> GetLatestVitals(int patientId)
        {
            var result = await _serviceManager.VitalSignService.GetLatestVitalsAsync(patientId);
            if (result is null) return NotFound($"No vital signs found for patient {patientId}.");
            return Ok(result);
        }
    }
}
