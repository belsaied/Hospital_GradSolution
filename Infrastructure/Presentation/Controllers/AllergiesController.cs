using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.PatientModule.AllergyDtos;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/patients/{patientId:int}/allergies")]
    public class AllergiesController(IServiceManager _serviceManager) : ControllerBase
    {
        // Add allergy for a patient
        [HttpPost]
        [ProducesResponseType(typeof(AllergyResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AllergyResultDto>> AddAllergy(int patientId, [FromBody] CreateAllergyDto allergyDto)
        {
            var allergy = await _serviceManager.AllergyService.AddAllergyAsync(patientId, allergyDto);

            if (allergy is null)
                return NotFound($"Patient with ID {patientId} not found.");

            return CreatedAtAction(nameof(GetPatientAllergies), new { patientId }, allergy);
        }

        // Get all allergies for a patient
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AllergyResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AllergyResultDto>>> GetPatientAllergies(int patientId)
        {
            var allergies = await _serviceManager.AllergyService.GetPatientAllergiesAsync(patientId);
            return Ok(allergies);
        }

        // Remove an allergy
        [HttpDelete("{allergyId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveAllergy(int patientId, int allergyId)
        {
            var result = await _serviceManager.AllergyService.RemoveAllergyAsync(patientId, allergyId);

            if (!result)
                return NotFound($"Allergy with ID {allergyId} not found for patient {patientId}.");

            return NoContent();
        }
    }
}
