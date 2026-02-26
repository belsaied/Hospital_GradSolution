using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared;
using Shared.Dtos.AppointmentModule;
using Shared.Parameters;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController (IServiceManager _serviceManager): ControllerBase
    {
        // POST /api/appointments
        [HttpPost]
        [ProducesResponseType(typeof(AppointmentResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<AppointmentResultDto>> BookAppointment(
            [FromBody] CreateAppointmentDto dto)
        {
            var result = await _serviceManager.AppointmentService.BookAppointmentAsync(dto);
            return CreatedAtAction(nameof(GetAppointmentById), new { id = result.Id }, result);
        }

        // GET /api/appointments/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AppointmentResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppointmentResultDto>> GetAppointmentById(int id)
            => Ok(await _serviceManager.AppointmentService.GetAppointmentByIdAsync(id));

        // GET /api/appointments
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<AppointmentResultDto>),
            StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<AppointmentResultDto>>>
            GetAllAppointments([FromQuery] AppointmentSpecificationParameters parameters)
            => Ok(await _serviceManager.AppointmentService
                    .GetAllAppointmentsAsync(parameters));

        // GET /api/appointments/patient/{patientId}
        [HttpGet("patient/{patientId:int}")]
        [ProducesResponseType(typeof(IEnumerable<AppointmentResultDto>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<AppointmentResultDto>>>
            GetPatientAppointments(int patientId)
            => Ok(await _serviceManager.AppointmentService
                    .GetPatientAppointmentsAsync(patientId));

        // GET /api/appointments/doctor/{doctorId}?date=2026-03-10
        [HttpGet("doctor/{doctorId:int}")]
        [ProducesResponseType(typeof(IEnumerable<AppointmentResultDto>),
            StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<AppointmentResultDto>>>
            GetDoctorAppointments(int doctorId, [FromQuery] DateOnly date)
            => Ok(await _serviceManager.AppointmentService
                    .GetDoctorAppointmentsAsync(doctorId, date));

        // GET /api/appointments/available-slots?doctorId=1&date=2026-03-10
        [HttpGet("available-slots")]
        [ProducesResponseType(typeof(IEnumerable<AvailableSlotDto>),
            StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AvailableSlotDto>>>
            GetAvailableSlots([FromQuery] int doctorId, [FromQuery] DateOnly date)
            => Ok(await _serviceManager.AppointmentService
                    .GetAvailableSlotsAsync(doctorId, date));

        // PUT /api/appointments/{id}/confirm
        [HttpPut("{id:int}/confirm")]
        [ProducesResponseType(typeof(AppointmentResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<AppointmentResultDto>> ConfirmAppointment(int id)
            => Ok(await _serviceManager.AppointmentService.ConfirmAppointmentAsync(id));

        // PUT /api/appointments/{id}/complete
        [HttpPut("{id:int}/complete")]
        [ProducesResponseType(typeof(AppointmentResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<AppointmentResultDto>> CompleteAppointment(
            int id, [FromQuery] string? notes = null)
            => Ok(await _serviceManager.AppointmentService
                    .CompleteAppointmentAsync(id, notes));

        // PUT /api/appointments/{id}/cancel
        [HttpPut("{id:int}/cancel")]
        [ProducesResponseType(typeof(AppointmentResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CancelAppointment(
            int id, [FromQuery] string reason)
        {
            await _serviceManager.AppointmentService.CancelAppointmentAsync(id, reason);
            return Ok(new { message = "Appointment cancelled successfully." });
        }

        // PUT /api/appointments/{id}/no-show
        [HttpPut("{id:int}/no-show")]
        [ProducesResponseType(typeof(AppointmentResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<AppointmentResultDto>> MarkNoShow(int id)
        {
            var dto = new UpdateAppointmentStatusDto
            { NewStatus = Domain.Models.Enums.AppointmentEnums.AppointmentStatus.NoShow };
            return Ok(await _serviceManager.AppointmentService
                .UpdateAppointmentStatusAsync(id, dto));
        }
    }
}
