using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Services.Abstraction.Contracts.NotificationService;
using Shared.Dtos.NotificationDtos.Requests;
using Shared.Dtos.NotificationDtos.Results;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Presentation.Controllers.NotificationModule
{
    [ApiController]
    [Route("api/notification-preferences")]
    [Authorize]
    public class NotificationPreferencesController(
       INotificationPreferenceService _preferenceService) : ControllerBase
    {
        private string CurrentUserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("patient_id")
            ?? User.FindFirstValue("doctor_id")
            ?? string.Empty;

        // GET /api/notification-preferences
        // Returns all preference settings for the current user
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NotificationPreferenceResult>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NotificationPreferenceResult>>> GetMyPreferences()
        {
            var result = await _preferenceService.GetPreferencesAsync(CurrentUserId);
            return Ok(result);
        }

        // PUT /api/notification-preferences
        // Enable or disable a specific type + channel combination
        [HttpPut]
        [ProducesResponseType(typeof(NotificationPreferenceResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NotificationPreferenceResult>> UpdatePreference(
            [FromBody] UpdatePreferenceRequest request)
        {
            var result = await _preferenceService.UpdatePreferenceAsync(CurrentUserId, request);
            return Ok(result);
        }

        // DELETE /api/notification-preferences/reset
        // Resets all preferences to default (all enabled — opt back in)
        [HttpDelete("reset")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> ResetPreferences()
        {
            await _preferenceService.ResetPreferencesToDefaultAsync(CurrentUserId);
            return NoContent();
        }
    }
}
