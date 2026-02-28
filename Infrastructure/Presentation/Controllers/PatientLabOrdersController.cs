using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.MedicalRecordsDto;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/patients/{patientId:int}/lab-orders")]
    public class PatientLabOrdersController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<LabOrderResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<LabOrderResultDto>>> GetPatientLabOrders(int patientId)
    => Ok(await _serviceManager.LabOrderService.GetPatientLabOrdersAsync(patientId));
    }
}
