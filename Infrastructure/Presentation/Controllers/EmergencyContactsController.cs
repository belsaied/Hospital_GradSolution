using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.PatientModule.EmergencyContactsDtos;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/patients/{patientId:int}/emergency-contacts")]
    public class EmergencyContactsController(IServiceManager _serviceManager) : ControllerBase
    {
        // Add emergency contact for a patient
        [HttpPost]
        [ProducesResponseType(typeof(EmergencyContactResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmergencyContactResultDto>> AddEmergencyContact(
            int patientId,
            [FromBody] CreateEmergencyContactDto contactDto)
        {
            var emergencyContact = await _serviceManager.EmergencyContactService.AddEmergencyContactAsync(patientId, contactDto);

            if (emergencyContact is null)
                return NotFound($"Patient with ID {patientId} not found.");

            return CreatedAtAction(nameof(GetEmergencyContacts), new { patientId }, emergencyContact);
        }

        // Get all emergency contacts for a patient
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<EmergencyContactResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EmergencyContactResultDto>>> GetEmergencyContacts(int patientId)
        {
            var contacts = await _serviceManager.EmergencyContactService.GetEmergencyContactsAsync(patientId);
            return Ok(contacts);
        }

        // Update emergency contact
        [HttpPut("{contactId:int}")]
        [ProducesResponseType(typeof(EmergencyContactResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<EmergencyContactResultDto>> UpdateEmergencyContact(
            int patientId,
            int contactId,
            [FromBody] UpdateEmergencyContactDto contactDto)
        {
            var emergencyContact = await _serviceManager.EmergencyContactService.UpdateEmergencyContactAsync(patientId, contactId, contactDto);

            if (emergencyContact is null)
                return NotFound($"Emergency contact with ID {contactId} not found for patient {patientId}.");

            return Ok(emergencyContact);
        }

        // Delete emergency contact
        [HttpDelete("{contactId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteEmergencyContact(int patientId, int contactId)
        {
            var result = await _serviceManager.EmergencyContactService.DeleteEmergencyContactAsync(patientId, contactId);

            if (!result)
                return NotFound($"Emergency contact with ID {contactId} not found for patient {patientId}.");

            return NoContent();
        }
    }
}
