using Domain.Contracts;
using Domain.Models.IdentityModule;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Persistence.Data.DbContexts;
using Persistence.Data.Identity;
using Persistence.Implementations;
using Persistence.Senders;
using QuestPDF.Infrastructure;
using Services.Abstraction.Contracts.NotificationService;
using Services.Implementations.BillingModule;
using Services.Implementations.NotificationModule.Jobs;
using Shared.Common;
using StackExchange.Redis;
using System.Text;

namespace Hospital_Grad.API.Extensions
{
    public static class InfrastructureServiceExtensions
    {
            public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
            {
            services.AddDbContext<HospitalDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
            services.AddDbContext<IdentityHospitalDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("IdentityConnection"));
            });
            services.AddScoped<IDataSeeding, DataSeeding>();
            services.AddScoped<IdentityDataSeeding>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IConnectionMultiplexer>(_ =>
             ConnectionMultiplexer.Connect(
            configuration.GetConnectionString("RedisConnection")!));

            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddScoped<IAuditRepository, AuditRepository>();
            QuestPDF.Settings.License = LicenseType.Community;
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireUppercase = true;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = true;
                options.User.RequireUniqueEmail = true;
            })
              .AddEntityFrameworkStores<IdentityHospitalDbContext>()
              .AddDefaultTokenProviders();
            services.ValidateJwt(configuration);
            services.Configure<EmailOptions>(configuration.GetSection("EmailOptions"));
            // QuestPDF License
            QuestPDF.Settings.License = LicenseType.Community;


            // ── Email sender — switched via appsettings EmailSettings:Provider ─
            var emailProvider = configuration["EmailSettings:Provider"] ?? "Smtp";
            if (emailProvider.Equals("SendGrid", StringComparison.OrdinalIgnoreCase))
                services.AddScoped<IEmailSender, SendGridEmailSender>();
            else
                services.AddScoped<IEmailSender, SmtpEmailSender>();


            // ── SMS sender — Twilio 
            services.AddScoped<ISmsSender, TwilioSmsSender>();

            // ── Push sender — SignalR wrapper 
            services.AddScoped<INotificationPushSender, NotificationPushSender>();

            // ── SignalR 
            services.AddSignalR();


            // Hangfire
            services.AddHangfire(config => config
                .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                .UseSimpleAssemblyNameTypeSerializer()
                .UseRecommendedSerializerSettings()
                .UseSqlServerStorage(
                    configuration.GetConnectionString("DefaultConnection"),
                    new SqlServerStorageOptions
                    {
                        CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                        SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                        QueuePollInterval = TimeSpan.Zero,
                        UseRecommendedIsolationLevel = true,
                        DisableGlobalLocks = true
                    }));

            services.AddHangfireServer();

            // Hangfire Background Job classes
            services.AddScoped<MarkOverdueInvoicesJob>();
            services.AddScoped<InvoiceExpiryNotificationJob>();
            // Notification Jobs
            services.AddTransient<AppointmentReminderJob>();
            services.AddTransient<PrescriptionExpiryWarningJob>();
            services.AddTransient<InvoiceOverdueReminderJob>();

            //Cach
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration.GetConnectionString("Redis");
            });

            services.AddScoped<ICacheRepository, CacheRepository>();

            return services;
            }
        public static IServiceCollection ValidateJwt(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection("JwtOptions").Get<JwtOptions>();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions!.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                                                   Encoding.UTF8.GetBytes(jwtOptions.SecretKey))
                };
            });

            services.AddAuthorization();
            return services;
        }


    }
}
