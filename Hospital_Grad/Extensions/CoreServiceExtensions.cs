using Microsoft.AspNetCore.Authorization;
using Presentation.Authorization;
using Presentation.Hubs;
using Services.Abstraction.Contracts;
using Services.Abstraction.Contracts.BillingService;
using Services.Abstraction.Contracts.NotificationService;
using Services.Abstraction.Contracts.WardBedService;
using Services.Implementations;
using Services.Implementations.AppointmentModule;
using Services.Implementations.BillingModule;
using Services.Implementations.DoctorModule;
using Services.Implementations.MedicalRecordModule;
using Services.Implementations.NotificationModule;
using Services.Implementations.NotificationModule.Jobs;
using Services.Implementations.PatientModule;
using Services.Implementations.UserManagementModule;
using Services.Implementations.WardBedModule;
using Shared.Common;
using Shared.Common.NotificationSettings;

namespace Hospital_Grad.API.Extensions
{
    public static class CoreServiceExtensions
    {
        public static IServiceCollection AddCoreServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(cfg => { },
                typeof(Services.ServiceAssemblyReference).Assembly);

            services.AddScoped<IServiceManager, ServiceManagerWithFactoryDelegate>();
            services.AddHttpContextAccessor();
            services.AddScoped<IAuthorizationHandler, PatientOwnershipHandler>();
            services.AddAuthorizationBuilder()
                .AddPolicy("PatientOwnership", policy =>
                    policy.Requirements.Add(new PatientOwnershipRequirement()));

            // ── Patient Module ────────────────────────────────────────────
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IAllergyService, AllergyService>();
            services.AddScoped<IMedicalHistoryService, MedicalHistoryService>();
            services.AddScoped<IEmergencyContactService, EmergencyContactService>();

            // ── Doctor Module ─────────────────────────────────────────────
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IAppointmentNotifier, AppointmentNotifier>();

            // ── Medical Record Module ─────────────────────────────────────
            services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            services.AddScoped<IVitalSignService, VitalSignService>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();
            services.AddScoped<ILabOrderService, LabOrderService>();

            // ── WardBed Module ────────────────────────────────────────────
            services.AddScoped<IWardService, WardService>();
            services.AddScoped<IBedService, BedService>();
            services.AddScoped<IAdmissionService, AdmissionService>();
            services.AddScoped<IBedNotifier, BedNotifier>();

            // ── Identity ──────────────────────────────────────────────────
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IEmailService, EmailService>();

            // ── Billing Module ────────────────────────────────────────────
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IInsuranceService, InsuranceService>();
            services.AddScoped<IInvoicePdfGenerator, InvoicePdfGenerator>();
            services.AddScoped<IReportingService, ReportingService>();
            services.AddScoped<IInvoiceNotifier, InvoiceNotifier>();

            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<INotificationPreferenceService, NotificationPreferenceService>();
            services.AddScoped<INotificationLogService, NotificationLogService>();
            services.AddScoped<IAdminNotificationLogService, AdminNotificationLogService>();

            // ── Cache ─────────────────────────────────────────────────────
            services.AddScoped<ICacheService, CacheService>();

            // ── Notification Hangfire Jobs ────────────────────────────────
            services.AddTransient<AppointmentReminderJob>();
            services.AddTransient<PrescriptionExpiryWarningJob>();
            services.AddTransient<InvoiceOverdueReminderJob>();

            // ── Factory delegates for ServiceManagerWithFactoryDelegate ───
            services.AddScoped<Func<IPatientService>>(p =>
                () => p.GetRequiredService<IPatientService>());
            services.AddScoped<Func<IAllergyService>>(p =>
                () => p.GetRequiredService<IAllergyService>());
            services.AddScoped<Func<IMedicalHistoryService>>(p =>
                () => p.GetRequiredService<IMedicalHistoryService>());
            services.AddScoped<Func<IEmergencyContactService>>(p =>
                () => p.GetRequiredService<IEmergencyContactService>());
            services.AddScoped<Func<IDoctorService>>(p =>
                () => p.GetRequiredService<IDoctorService>());
            services.AddScoped<Func<IDepartmentService>>(p =>
                () => p.GetRequiredService<IDepartmentService>());
            services.AddScoped<Func<IAppointmentService>>(p =>
                () => p.GetRequiredService<IAppointmentService>());
            services.AddScoped<Func<IMedicalRecordService>>(p =>
                () => p.GetRequiredService<IMedicalRecordService>());
            services.AddScoped<Func<IVitalSignService>>(p =>
                () => p.GetRequiredService<IVitalSignService>());
            services.AddScoped<Func<IPrescriptionService>>(p =>
                () => p.GetRequiredService<IPrescriptionService>());
            services.AddScoped<Func<ILabOrderService>>(p =>
                () => p.GetRequiredService<ILabOrderService>());
            services.AddScoped<Func<IWardService>>(p =>
                () => p.GetRequiredService<IWardService>());
            services.AddScoped<Func<IBedService>>(p =>
                () => p.GetRequiredService<IBedService>());
            services.AddScoped<Func<IAdmissionService>>(p =>
                () => p.GetRequiredService<IAdmissionService>());
            services.AddScoped<Func<IAuthService>>(p =>
                () => p.GetRequiredService<IAuthService>());
            services.AddScoped<Func<IAuditService>>(p =>
                () => p.GetRequiredService<IAuditService>());
            services.AddScoped<Func<IEmailService>>(p =>
                () => p.GetRequiredService<IEmailService>());
            services.AddScoped<Func<IInvoiceService>>(p =>
                () => p.GetRequiredService<IInvoiceService>());
            services.AddScoped<Func<IPaymentService>>(p =>
                () => p.GetRequiredService<IPaymentService>());
            services.AddScoped<Func<IInsuranceService>>(p =>
                () => p.GetRequiredService<IInsuranceService>());
            services.AddScoped<Func<IReportingService>>(p =>
                () => p.GetRequiredService<IReportingService>());
            services.AddScoped<Func<IInvoicePdfGenerator>>(p =>
                () => p.GetRequiredService<IInvoicePdfGenerator>());
            services.AddScoped<Func<IInvoiceNotifier>>(p =>
                () => p.GetRequiredService<IInvoiceNotifier>());
            services.AddScoped<Func<INotificationService>>(p =>
                () => p.GetRequiredService<INotificationService>());
            services.AddScoped<Func<INotificationPreferenceService>>(p =>
                () => p.GetRequiredService<INotificationPreferenceService>());
            services.AddScoped<Func<INotificationLogService>>(p =>
                () => p.GetRequiredService<INotificationLogService>());
            services.AddScoped<Func<IAdminNotificationLogService>>(p =>
                () => p.GetRequiredService<IAdminNotificationLogService>());
            services.AddScoped<Func<ICacheService>>(p =>
                () => p.GetRequiredService<ICacheService>());

            // ── Configuration binding ─────────────────────────────────────
            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
            services.Configure<NotificationEmailSettings>(
                configuration.GetSection("NotificationEmailSettings"));
            services.Configure<TwilioSettings>(
                configuration.GetSection("TwilioSettings"));

            return services;
        }
    }
}
