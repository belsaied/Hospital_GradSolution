using Presentation.Hubs;
using Services;
using Services.Abstraction.Contracts;
using Services.Abstraction.Contracts.WardBedService;
using Services.Implementations;
using Services.Implementations.AppointmentModule;
using Services.Implementations.DoctorModule;
using Services.Implementations.MedicalRecordModule;
using Services.Implementations.PatientModule;
using Services.Implementations.WardBedModule;

namespace Hospital_Grad.API.Extensions
{
    public static class CoreServiceExtensions
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => { }, typeof(ServiceAssemblyReference).Assembly);
            services.AddScoped<IServiceManager, ServiceManagerWithFactoryDelegate>();

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
            return services;
        }
    }
}
