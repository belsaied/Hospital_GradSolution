using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.MedicalRecordsDto;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/patients/{patientId:int}/lab-orders")]
    [Authorize]
    public class PatientLabOrdersController(IServiceManager _serviceManager) : ControllerBase
    {
        [Authorize(Policy = "PatientOwnership")]
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LabOrderResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LabOrderResultDto>>> GetPatientLabOrders(int patientId)
    => Ok(await _serviceManager.LabOrderService.GetPatientLabOrdersAsync(patientId));
    }
}
