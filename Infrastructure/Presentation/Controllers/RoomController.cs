using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.WardBedModule.BedDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomController(IServiceManager _serviceManager) : ControllerBase
    {
        // GET api/rooms/1/beds
        [HttpGet("{roomId:int}/beds")]
        [ProducesResponseType(typeof(IEnumerable<BedResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBedsInRoom(int roomId)
        {
            var result = await _serviceManager.BedService.GetBedsInRoomAsync(roomId);
            return Ok(result);
        }

        // POST api/rooms/1/beds
        [HttpPost("{roomId:int}/beds")]
        [ProducesResponseType(typeof(BedResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> AddBedToRoom(int roomId, [FromBody] CreateBedDto dto)
        {
            var result = await _serviceManager.BedService.AddBedToRoomAsync(roomId, dto);
            return CreatedAtAction(nameof(GetBedsInRoom), new { roomId }, result);
        }
        
    }
}
