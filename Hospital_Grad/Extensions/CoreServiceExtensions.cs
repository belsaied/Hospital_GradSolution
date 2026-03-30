using Microsoft.AspNetCore.Authorization;
using Presentation.Authorization;
using Presentation.Hubs;
using Services;
using Services.Abstraction.Contracts;
using Services.Abstraction.Contracts.BillingService;
using Services.Abstraction.Contracts.WardBedService;
using Services.Implementations;
using Services.Implementations.AppointmentModule;
using Services.Implementations.BillingModule;
using Services.Implementations.DoctorModule;
using Services.Implementations.MedicalRecordModule;
using Services.Implementations.PatientModule;
using Services.Implementations.UserManagementModule;
using Services.Implementations.WardBedModule;
using Shared.Common;

namespace Hospital_Grad.API.Extensions
{
    public static class CoreServiceExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services , IConfiguration configuration)
        {
            services.AddAutoMapper(cfg => { }, typeof(ServiceAssemblyReference).Assembly);
            services.AddScoped<IServiceManager, ServiceManagerWithFactoryDelegate>();
            services.AddHttpContextAccessor();
            services.AddScoped<IAuthorizationHandler, PatientOwnershipHandler>();

            services.AddAuthorizationBuilder()
                .AddPolicy("PatientOwnership", policy =>
                    policy.Requirements.Add(new PatientOwnershipRequirement()));
            // Register all services
            services.AddScoped<IPatientService, PatientService>();
            services.AddScoped<IAllergyService, AllergyService>();
            services.AddScoped<IMedicalHistoryService, MedicalHistoryService>();
            services.AddScoped<IEmergencyContactService, EmergencyContactService>();
            services.AddScoped<IDoctorService, DoctorService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IAppointmentService, AppointmentService>();
            services.AddScoped<IAppointmentNotifier, AppointmentNotifier>();
            services.AddScoped<IMedicalRecordService, MedicalRecordService>();
            services.AddScoped<IVitalSignService, VitalSignService>();
            services.AddScoped<IPrescriptionService, PrescriptionService>();
            services.AddScoped<ILabOrderService, LabOrderService>();
            services.AddScoped<IWardService, WardService>();
            services.AddScoped<IBedService, BedService>();
            services.AddScoped<IAdmissionService, AdmissionService>();
            services.AddScoped<IBedNotifier, BedNotifier>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IAuditService, AuditService>();
            services.AddScoped<IEmailService, EmailService>();
            //  --------------------------------------------------------------------------
            // Billing Services
            services.AddScoped<IInvoiceService, InvoiceService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IInsuranceService, InsuranceService>();
            services.AddScoped<IInvoicePdfGenerator, InvoicePdfGenerator>();
            services.AddScoped<IReportingService,ReportingService>();
            services.AddScoped<IInvoiceNotifier, InvoiceNotifier>();
            // --------------------------------------------------------------------------
            // Redis Caching
            services.AddScoped<ICacheService, CacheService>();
            // Register factory delegates
            services.AddScoped<Func<IPatientService>>(provider =>
                () => provider.GetRequiredService<IPatientService>()
            );
            services.AddScoped<Func<IAllergyService>>(provider =>
                () => provider.GetRequiredService<IAllergyService>()
            );
            services.AddScoped<Func<IMedicalHistoryService>>(provider =>
                () => provider.GetRequiredService<IMedicalHistoryService>()
            );
            services.AddScoped<Func<IEmergencyContactService>>(provider =>
                () => provider.GetRequiredService<IEmergencyContactService>()
            );
            services.AddScoped<Func<IDoctorService>>(provider => 
                ()=> provider.GetRequiredService<IDoctorService>()
            ); 
            services.AddScoped<Func<IDepartmentService>>(provider =>
                () => provider.GetRequiredService<IDepartmentService>()
            );
            services.AddScoped<Func<IAppointmentService>>(provider =>
                () => provider.GetRequiredService<IAppointmentService>()
            );
            services.AddScoped<Func<IMedicalRecordService>>(provider => 
            () => provider.GetRequiredService<IMedicalRecordService>()
            );
            services.AddScoped<Func<IVitalSignService>>(provider => 
            () => provider.GetRequiredService<IVitalSignService>()
            );
            services.AddScoped<Func<IPrescriptionService>>(provider =>
            () => provider.GetRequiredService<IPrescriptionService>()
            );
            services.AddScoped<Func<ILabOrderService>>(provider =>
            () => provider.GetRequiredService<ILabOrderService>()
            );
            services.AddScoped<Func<IWardService>>(provider =>
            () => provider.GetRequiredService<IWardService>()
            );
            services.AddScoped<Func<IBedService>>(provider =>
            () => provider.GetRequiredService<IBedService>()
            ); 
            services.AddScoped<Func<IAdmissionService>>(provider =>
            () => provider.GetRequiredService<IAdmissionService>()
            );
            services.AddScoped<Func<IAuthService>>(p => () => p.GetRequiredService<IAuthService>());
            services.AddScoped<Func<IAuditService>>(p => () => p.GetRequiredService<IAuditService>());
            services.AddScoped<Func<IEmailService>>(p => () => p.GetRequiredService<IEmailService>());
            services.AddScoped<Func<IInvoiceService>>(p => () => p.GetRequiredService<IInvoiceService>());
            services.AddScoped<Func<IPaymentService>>(p => () => p.GetRequiredService<IPaymentService>());
            services.AddScoped<Func<IInsuranceService>>(p => () => p.GetRequiredService<IInsuranceService>());
            services.AddScoped<Func<IReportingService>>(p => () => p.GetRequiredService<IReportingService>());
            services.AddScoped<Func<IInvoicePdfGenerator>>(p => () => p.GetRequiredService<IInvoicePdfGenerator>());
            services.AddScoped<Func<IInvoiceNotifier>>(p => () => p.GetRequiredService<IInvoiceNotifier>());
            services.AddScoped<Func<ICacheService>>(p => () => p.GetRequiredService<ICacheService>());

            services.Configure<JwtOptions>(configuration.GetSection("JwtOptions"));
            return services;
        }
    }
}
