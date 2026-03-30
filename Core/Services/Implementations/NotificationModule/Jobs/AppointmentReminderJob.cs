using Domain.Contracts;
using Domain.Models.AppointmentModule;
using Domain.Models.Enums.AppointmentEnums;
using Domain.Models.Enums.NotificationEnums;
using Domain.Models.NotificationModule;
using Microsoft.Extensions.Logging;
using Services.Abstraction.Contracts.NotificationService;
using Services.Specifications.AppointmentModule;
using Services.Specifications.NotificationModule.NotificationSpecification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Implementations.NotificationModule.Jobs
{
    public sealed class AppointmentReminderJob(
        IUnitOfWork _unitOfWork,
        INotificationService _notificationService,
        ILogger<AppointmentReminderJob> _logger)
    {
        public async Task ExecuteAsync()
        {
            var tomorrow = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1));

            var aptRepo = _unitOfWork.GetRepository<Appointment, int>();
            var appointments = (await aptRepo.GetAllAsync(
                new AppointmentListSpecification(new Shared.Parameters.AppointmentSpecificationParameters
                {
                    Status = AppointmentStatus.Confirmed,
                    FromDate = tomorrow,
                    ToDate = tomorrow,
                    PageSize = 500,
                    PageIndex = 1
                }))).ToList();

            if (!appointments.Any())
            {
                _logger.LogInformation("[AppointmentReminderJob] No confirmed appointments for {Date}.", tomorrow);
                return;
            }

            var notifRepo = _unitOfWork.GetRepository<Notification, Guid>();
            var today = DateTimeOffset.UtcNow.Date;
            int sent = 0;

            foreach (var apt in appointments)
            {
                // Deduplication: skip if reminder already sent today for this appointment
                var alreadySent = await notifRepo.CountAsync(
                    new NotificationLogForDeduplicationSpec(
                        apt.Id.ToString(),
                        NotificationType.AppointmentReminder,
                        lookBackDays: 1)) > 0;

                if (alreadySent)
                {
                    _logger.LogDebug("[AppointmentReminderJob] Reminder already sent for appointment {Id}.", apt.Id);
                    continue;
                }

                try
                {
                    await _notificationService.SendAppointmentReminderAsync(apt.PatientId, apt.Id);
                    sent++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "[AppointmentReminderJob] Failed for appointment {Id}.", apt.Id);
                }
            }

            _logger.LogInformation("[AppointmentReminderJob] Sent {Count} reminders for {Date}.", sent, tomorrow);
        }
    }
}
