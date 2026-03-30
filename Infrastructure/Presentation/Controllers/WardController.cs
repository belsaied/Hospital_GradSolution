using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Authorization;
using Services.Abstraction.Contracts;
using Shared.Dtos.WardBedModule.RoomsDtos;
using Shared.Dtos.WardBedModule.WardDtos;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/wards")]
    [Authorize]
    public class WardController(IServiceManager _serviceManager) : ControllerBase
    {
        // POST api/wards
        [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
        [HttpPost]
        [ProducesResponseType(typeof(WardResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateWard([FromBody] CreateWardDto dto)
        {
            var result = await _serviceManager.WardService.CreateWardAsync(dto);
            return CreatedAtAction(nameof(GetWardById), new { id = result.Id }, result);
        }

        // GET api/wards
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Doctor,Nurse,Receptionist")]
        [HttpGet]
        [RedisCache(durationInSeconds: 60)]
        [ProducesResponseType(typeof(IEnumerable<WardOccupancySummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllWards()
        {
            var result = await _serviceManager.WardService.GetAllWardsWithOccupancyAsync();
            return Ok(result);
        }

        // GET api/wards/{id}
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Doctor,Nurse,Receptionist")]
        [HttpGet("{id:int}")]
        [RedisCache(durationInSeconds: 300)]
        [ProducesResponseType(typeof(WardWithDetailsResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWardById(int id)
        {
            var result = await _serviceManager.WardService.GetWardByIdAsync(id);
            return Ok(result);
        }

        // PUT api/wards/{id}
        [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(WardResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateWard(int id, [FromBody] UpdateWardDto dto)
        {
            var result = await _serviceManager.WardService.UpdateWardAsync(id, dto);
            return Ok(result);
        }

        // POST api/wards/{wardId}/rooms
        [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
        [HttpPost("{wardId:int}/rooms")]
        [ProducesResponseType(typeof(RoomResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddRoom(int wardId, [FromBody] CreateRoomDto dto)
        {
            var result = await _serviceManager.WardService.AddRoomToWardAsync(wardId, dto);
            return CreatedAtAction(nameof(GetRoomsInWard), new { wardId }, result);
        }

        // GET api/wards/{wardId}/rooms
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Doctor,Nurse,Receptionist")]
        [HttpGet("{wardId:int}/rooms")]
        [RedisCache]
        [ProducesResponseType(typeof(IEnumerable<RoomResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetRoomsInWard(int wardId)
        {
            var result = await _serviceManager.WardService.GetRoomsInWardAsync(wardId);
            return Ok(result);
        }
    }
}
