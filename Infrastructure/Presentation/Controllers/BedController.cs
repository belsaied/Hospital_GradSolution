using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Authorization;
using Services.Abstraction.Contracts;
using Shared.Dtos.WardBedModule.BedDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/beds")]
    [Authorize]
    public class BedController(IServiceManager _serviceManager) : ControllerBase
    {
        // GET api/beds/available
        // GET api/beds/available?wardType=ICU&bedType=Standard
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Doctor,Nurse,Receptionist")]
        [HttpGet("available")]
        [RedisCache(durationInSeconds: 60)]
        [ProducesResponseType(typeof(IEnumerable<BedAvailabilityResultDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailableBeds(
            [FromQuery] string? wardType = null,
            [FromQuery] string? bedType = null)
        {
            var result = await _serviceManager.BedService.GetAvailableBedsAsync(wardType, bedType);
            return Ok(result);
        }
        // PUT api/beds/5/status
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Nurse")]
        [HttpPut("{bedId:int}/status")]
        [ProducesResponseType(typeof(BedResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> UpdateBedStatus(int bedId, [FromBody] UpdateBedStatusDto dto)
        {
            var result = await _serviceManager.BedService.UpdateBedStatusAsync(bedId, dto);
            return Ok(result);
        }
        
    }
}
