using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.WardBedModule.BedDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/beds")]
    public class BedController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpPost("{bedId:int}/beds")]
        public async Task<IActionResult> AddBed(int bedId, [FromBody] CreateBedDto dto)
        {
            var result = await _serviceManager.BedService.AddBedToRoomAsync(bedId, dto);
            return CreatedAtAction(nameof(GetBedsInRoom), new { bedId }, result);
        }

        [HttpGet("{bedId:int}/beds")]
        public async Task<IActionResult> GetBedsInRoom(int bedId)
        {
            var result = await _serviceManager.BedService.GetBedsInRoomAsync(bedId);
            return Ok(result);
        }

        [HttpPatch("beds/{bedId:int}/status")]
        public async Task<IActionResult> UpdateBedStatus(int bedId, [FromBody] UpdateBedStatusDto dto)
        {
            var result = await _serviceManager.BedService.UpdateBedStatusAsync(bedId, dto);
            return Ok(result);
        }

        [HttpGet("beds/available")]
        public async Task<IActionResult> GetAvailableBeds(
        [FromQuery] string? wardType = null,
        [FromQuery] string? bedType = null)
        {
            var result = await _serviceManager.BedService.GetAvailableBedsAsync(wardType, bedType);
            return Ok(result);
        }
    }
}
