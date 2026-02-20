using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared;
using Shared.Dtos.PatientModule.PatientDtos;
using Shared.Parameters;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController(IServiceManager _serviceManager) : ControllerBase
    {
        // Register a new patient
        [HttpPost]
        [ProducesResponseType(typeof(PatientResultDto), StatusCodes.Status201Created)]
        public async Task<ActionResult<PatientResultDto>> RegisterPatient([FromBody] CreatePatientDto createPatientDto)
        {
            var patient = await _serviceManager.PatientService.RegisterPatientAsync(createPatientDto);
            return CreatedAtAction(nameof(GetPatientById), new { id = patient.Id }, patient);
        }
        // Get patient details by ID
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(PatientResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientResultDto>> GetPatientById(int id)
         => Ok(await _serviceManager.PatientService.GetPatientByIdAsync(id));

        // Update patient
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(PatientResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatientResultDto>> UpdatePatient(int id, [FromBody] UpdatePatientDto updatePatientDto)
          => Ok(await _serviceManager.PatientService.UpdatePatientAsync(id, updatePatientDto));

        // Deactivate patient (soft delete)
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivatePatient(int id)
        {
            await _serviceManager.PatientService.DeactivatePatientAsync(id);
            return NoContent();
        }


       

        // GET api/patients — paginated list
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<PatientResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<PatientResultDto>>> GetAllPatients(
            [FromQuery] PatientSpecificationParameters parameters)
        {
            var result = await _serviceManager.PatientService.GetAllPatientsAsync(parameters);
            return Ok(result);
        }

        // GET api/patients/{id}/details — single patient with full nested data
        [HttpGet("{id:int}/details")]
        [ProducesResponseType(typeof(PatientWithDetailsResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientWithDetailsResultDto>> GetPatientWithDetails(int id)
        {
            var patient = await _serviceManager.PatientService.GetPatientWithDetailsAsync(id);
            if (patient is null)
                return NotFound($"Patient with ID {id} not found.");
            return Ok(patient);
        }
    }
}
