using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Presentation.Authorization;
using Services.Abstraction.Contracts;
using Shared;
using Shared.Dtos.DoctorModule.DoctorDtos;
using Shared.Parameters;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DoctorsController(IServiceManager _serviceManager) : ControllerBase
    {
        //Post  /api/doctors
        [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
        [HttpPost]
        [ProducesResponseType(typeof(DoctorResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<DoctorResultDto>> RegisterDoctor([FromBody] CreateDoctorDto createDoctorDto)
        {
            var doctor = await _serviceManager.DoctorService.RegisterDoctorAsync(createDoctorDto);
            return CreatedAtAction(nameof(GetDoctorById), new { id = doctor.Id }, doctor);
        }

        //Get  /api/doctors/{id}
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Receptionist,Doctor,Patient")]
        [HttpGet("{id:int}")]
        [RedisCache(durationInSeconds: 300)]
        [ProducesResponseType(typeof(DoctorResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorResultDto>> GetDoctorById(int id)
                    => Ok(await _serviceManager.DoctorService.GetDoctorByIdAsync(id));



        //Put  api/doctors/{id}
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Doctor")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(DoctorResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorResultDto>> UpdateDoctor(int id, [FromBody] UpdateDoctorDto updateDoctorDto)
                    => Ok(await _serviceManager.DoctorService.UpdateDoctorAsync(id, updateDoctorDto));


        //Delete /api/doctors/{id} =>  Soft delete
        [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> DeactivateDoctor(int id)
        {
            await _serviceManager.DoctorService.DeactivateDoctorAsync(id);
            return NoContent();
        }


        //Get  /api/doctors
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Receptionist,Patient")]
        [HttpGet]
        [RedisCache(durationInSeconds: 180)]
        [ProducesResponseType(typeof(PaginatedResult<DoctorResultDto>) , StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<DoctorResultDto>>> GetAllDoctors([FromQuery] DoctorSpecificationParameters parameters)
                  => Ok(await _serviceManager.DoctorService.GetAllDoctorsAsync(parameters));


        //Get /api/doctors/{id}/details
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Doctor")]
        [HttpGet("{id:int}/details")]
        [RedisCache(durationInSeconds: 300)]
        [ProducesResponseType(typeof(DoctorWithDetailsResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorWithDetailsResultDto>> GetDoctorWithDetails(int id)
            => Ok(await _serviceManager.DoctorService.GetDoctorWithDetailsAsync(id));

        //Get /api/doctors/department/{deptId}
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Receptionist,Patient")]
        [HttpGet("department/{deptId:int}")]
        [RedisCache(durationInSeconds: 180)]
        [ProducesResponseType(typeof(IEnumerable<DoctorResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<DoctorResultDto>>> GetDoctorsByDepartment(int deptId)
            => Ok(await _serviceManager.DoctorService.GetDoctorByDepartmentAsync(deptId));


        // GET /api/doctors/available?date=2026-02-23
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Receptionist,Patient")]
        [HttpGet("available")]
        [ProducesResponseType(typeof(IEnumerable<DoctorResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DoctorResultDto>>> GetAvailableDoctors(
            [FromQuery] DateTime date)
            => Ok(await _serviceManager.DoctorService.GetAvailableDoctorAsync(date));


        // POST /api/doctors/{id}/qualifications
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Doctor")]
        [HttpPost("{id:int}/qualifications")]
        [ProducesResponseType(typeof(QualificationResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<QualificationResultDto>> AddQualification(
            int id, [FromBody] CreateQualificationDto dto)
        {
            var qualification = await _serviceManager.DoctorService.AddQualificationAsync(id, dto);
            return CreatedAtAction(nameof(GetDoctorWithDetails), new { id }, qualification);
        }


        // DELETE /api/doctors/{id}/qualifications/{qId}
        [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
        [HttpDelete("{id:int}/qualifications/{qId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveQualification(int id, int qId)
        {
            await _serviceManager.DoctorService.RemoveQualificationAsync(id, qId);
            return NoContent();
        }

        // POST /api/doctors/{id}/schedules
        [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
        [HttpPost("{id:int}/schedules")]
        [ProducesResponseType(typeof(ScheduleResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ScheduleResultDto>> SetSchedule(
            int id, [FromBody] CreateScheduleDto dto)
        {
            var schedule = await _serviceManager.DoctorService.SetScheduleAsync(id, dto);
            return CreatedAtAction(nameof(GetSchedules), new { id }, schedule);
        }

        // GET /api/doctors/{id}/schedules
        [Authorize(Roles = "SuperAdmin,HospitalAdmin,Receptionist")]
        [HttpGet("{id:int}/schedules")]
        [RedisCache(durationInSeconds: 300)]
        [ProducesResponseType(typeof(IEnumerable<ScheduleResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<ScheduleResultDto>>> GetSchedules(int id)
            => Ok(await _serviceManager.DoctorService.GetScheduleAsync(id));
    }
}
