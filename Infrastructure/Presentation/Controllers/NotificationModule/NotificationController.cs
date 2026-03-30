using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Services.Abstraction.Contracts.NotificationService;
using Shared;
using Shared.Dtos.NotificationDtos.Requests;
using Shared.Dtos.NotificationDtos.Results;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace Presentation.Controllers.NotificationModule
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController(
        INotificationLogService _logService,
        IAdminNotificationLogService _adminLogService) : ControllerBase
    {
        private string CurrentUserId =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("patient_id")
            ?? User.FindFirstValue("doctor_id")
            ?? string.Empty;

        // GET /api/notifications
        // Returns paginated notification log for the currently authenticated user
        [HttpGet]
        [ProducesResponseType(typeof(PaginatedResult<NotificationLogResult>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<NotificationLogResult>>> GetMyNotifications(
            [FromQuery] NotificationLogFilter filter)
        {
            var result = await _logService.GetNotificationsByUserAsync(CurrentUserId, filter);
            return Ok(result);
        }

        // GET /api/notifications/unread
        // Returns all unread in-app Push notifications for the current user
        [HttpGet("unread")]
        [ProducesResponseType(typeof(IEnumerable<UnreadPushNotificationResult>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<UnreadPushNotificationResult>>> GetUnreadPush()
        {
            var result = await _logService.GetUnreadPushNotificationsAsync(CurrentUserId);
            return Ok(result);
        }

        // PUT /api/notifications/{id}/read
        // Marks a single push notification as read
        [HttpPut("{id:guid}/read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            await _logService.MarkPushNotificationReadAsync(id);
            return NoContent();
        }

        // PUT /api/notifications/read-all
        // Marks all push notifications for the current user as read
        [HttpPut("read-all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarkAllAsRead()
        {
            await _logService.MarkAllPushNotificationsReadAsync(CurrentUserId);
            return NoContent();
        }

        // GET /api/notifications/unread/count
        // Quick badge count of unread push notifications
        [HttpGet("unread/count")]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var count = await _logService.GetUnreadPushCountAsync(CurrentUserId);
            return Ok(count);
        }

        // GET /api/notifications/admin
        // Full paginated log across ALL recipients — SuperAdmin/HospitalAdmin only
        [HttpGet("admin")]
        [Authorize(Roles = "SuperAdmin,HospitalAdmin")]
        [ProducesResponseType(typeof(PaginatedResult<NotificationLogResult>), StatusCodes.Status200OK)]
        public async Task<ActionResult<PaginatedResult<NotificationLogResult>>> GetAdminLog(
            [FromQuery] NotificationLogFilter filter,
            [FromQuery] string? userId = null)
        {
            if (!string.IsNullOrEmpty(userId))
            {
                var userResult = await _logService.GetNotificationsByUserAsync(userId, filter);
                return Ok(userResult);
            }

            var result = await _adminLogService.GetAllNotificationsAsync(filter);
            return Ok(result);
        }
    }
}
