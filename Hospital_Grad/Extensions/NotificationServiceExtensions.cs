using Persistence.Senders;
using Presentation.Hubs;
using Services.Abstraction.Contracts.NotificationService;
using Services.Implementations.NotificationModule;
using Services.Implementations.NotificationModule.Jobs;
using Shared.Common.NotificationSettings;

namespace Hospital_Grad.API.Extensions
{
    public static class NotificationServiceExtensions
    {
        public static IServiceCollection AddNotificationServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // ── Settings ──────────────────────────────────────────────────────
            services.Configure<NotificationEmailSettings>(
                configuration.GetSection("NotificationEmailSettings"));

            services.Configure<TwilioSettings>(
                configuration.GetSection("TwilioSettings"));

            // ── Email Sender (provider-switched) ──────────────────────────────
            var provider = configuration["NotificationEmailSettings:Provider"] ?? "Smtp";
            if (provider.Equals("SendGrid", StringComparison.OrdinalIgnoreCase))
                services.AddScoped<IEmailSender, SendGridEmailSender>();
            else
                services.AddScoped<IEmailSender, SmtpEmailSender>();

            // ── SMS Sender ────────────────────────────────────────────────────
            services.AddScoped<ISmsSender, TwilioSmsSender>();

            // ── Push Sender (SignalR) ─────────────────────────────────────────
            services.AddScoped<INotificationPushSender, NotificationPushSender>();

            // ── Core Services ─────────────────────────────────────────────────
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<INotificationPreferenceService, NotificationPreferenceService>();
            services.AddScoped<INotificationLogService, NotificationLogService>();

            // ── Factory Delegates for ServiceManagerWithFactoryDelegate ────────
            services.AddScoped<Func<INotificationService>>(p =>
                () => p.GetRequiredService<INotificationService>());
            services.AddScoped<Func<INotificationPreferenceService>>(p =>
                () => p.GetRequiredService<INotificationPreferenceService>());
            services.AddScoped<Func<INotificationLogService>>(p =>
                () => p.GetRequiredService<INotificationLogService>());

            // ── Hangfire Jobs (transient = fresh DbContext per execution) ──────
            services.AddTransient<AppointmentReminderJob>();
            services.AddTransient<PrescriptionExpiryWarningJob>();
            services.AddTransient<InvoiceOverdueReminderJob>();

            // ── Seeding ───────────────────────────────────────────────────────
            services.AddScoped<DataSeeding>();

            return services;
        }
    }
}
