using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.MedicalRecordsDto;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/lab-orders")]
    public class LabOrdersController (IServiceManager _serviceManager) : ControllerBase
    {
        [HttpPut("{orderId:int}/status")]
        [ProducesResponseType(typeof(LabOrderResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<LabOrderResultDto>> UpdateLabOrderStatus(
    int orderId, [FromBody] UpdateLabOrderStatusDto dto)
    => Ok(await _serviceManager.LabOrderService.UpdateLabOrderStatusAsync(orderId, dto));

        [HttpPost("{orderId:int}/result")]
        [ProducesResponseType(typeof(LabResultResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<LabResultResultDto>> AddLabResult(
            int orderId, [FromBody] CreateLabResultDto dto)
        {
            var result = await _serviceManager.LabOrderService.AddLabResultAsync(orderId, dto);
            return CreatedAtAction(nameof(AddLabResult), new { orderId }, result);
        }
    }
}
