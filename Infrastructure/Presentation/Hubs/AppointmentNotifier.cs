using Microsoft.AspNetCore.SignalR;
using Services.Abstraction.Contracts;

namespace Presentation.Hubs
{
    public class AppointmentNotifier (IHubContext<AppointmentHub> _hubContext): IAppointmentNotifier
    {
        public async Task NotifyDoctorAsync(int doctorId, string eventName, object payload)
    => await _hubContext.Clients
        .Group($"doctor-{doctorId}")
        .SendAsync(eventName, payload);

        public async Task NotifyPatientAsync(int patientId, string eventName, object payload)
            => await _hubContext.Clients
                .Group($"patient-{patientId}")
                .SendAsync(eventName, payload);
    }
}
