using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.WardBedModule.RoomsDtos;
using Shared.Dtos.WardBedModule.WardDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/wards")]
    public class WardController(IServiceManager _serviceManager) : ControllerBase
    {
        [HttpPost]
        public async Task<IActionResult> CreateWard([FromBody] CreateWardDto dto)
        {
            var result = await _serviceManager.WardService.CreateWardAsync(dto);
            return CreatedAtAction(nameof(GetWardById), new { id = result.Id }, result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWards()
        {
            var result = await _serviceManager.WardService.GetAllWardsWithOccupancyAsync();
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetWardById(int id)
        {
            var result = await _serviceManager.WardService.GetWardByIdAsync(id);
            return Ok(result);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateWard(int id, [FromBody] UpdateWardDto dto)
        {
            var result = await _serviceManager.WardService.UpdateWardAsync(id, dto);
            return Ok(result);
        }

        [HttpPost("{wardId:int}/rooms")]
        public async Task<IActionResult> AddRoom(int wardId, [FromBody] CreateRoomDto dto)
        {
            var result = await _serviceManager.WardService.AddRoomToWardAsync(wardId, dto);
            return CreatedAtAction(nameof(GetRoomsInWard), new { wardId }, result);
        }

        [HttpGet("{wardId:int}/rooms")]
        public async Task<IActionResult> GetRoomsInWard(int wardId)
        {
            var result = await _serviceManager.WardService.GetRoomsInWardAsync(wardId);
            return Ok(result);
        }


    }
}
