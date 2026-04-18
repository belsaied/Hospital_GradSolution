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
using Shared.Common;
using StackExchange.Redis;
using System.Text;

namespace Hospital_Grad.API.Extensions
{
    public static class InfrastructureServiceExtensions
    {
        public static IServiceCollection AddInfrastructureServices(
            this IServiceCollection services, IConfiguration configuration)
        {
            // ── Database contexts ─────────────────────────────────────────
            services.AddDbContext<HospitalDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));

            services.AddDbContext<IdentityHospitalDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("IdentityConnection")));

            // ── Seeding ───────────────────────────────────────────────────
            services.AddScoped<IDataSeeding, DataSeeding>();
            services.AddScoped<IdentityDataSeeding>();

            // ── Repository / UoW ──────────────────────────────────────────
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // ── Redis ─────────────────────────────────────────────────────
            services.AddSingleton<IConnectionMultiplexer>(_ =>
                ConnectionMultiplexer.Connect(
                    configuration.GetConnectionString("RedisConnection")!));

            services.AddScoped<ICacheRepository, CacheRepository>();  // single registration
            services.AddScoped<IAuditRepository, AuditRepository>();

            // ── QuestPDF (community licence — set ONCE) ───────────────────
            QuestPDF.Settings.License = LicenseType.Community;

            // ── Identity ──────────────────────────────────────────────────
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

            services.Configure<EmailOptions>(
                configuration.GetSection("EmailOptions"));

            var emailProvider =
                configuration["NotificationEmailSettings:Provider"] ?? "Smtp";

            if (emailProvider.Equals("SendGrid", StringComparison.OrdinalIgnoreCase))
                services.AddScoped<IEmailSender, SendGridEmailSender>();
            else
                services.AddScoped<IEmailSender, SmtpEmailSender>();

            // ── SMS sender ────────────────────────────────────────────────
            services.AddScoped<ISmsSender, TwilioSmsSender>();

            // ── Push sender (SignalR wrapper) ─────────────────────────────
            services.AddScoped<INotificationPushSender, NotificationPushSender>();

            // ── SignalR ───────────────────────────────────────────────────
            services.AddSignalR();

            // ── Hangfire ──────────────────────────────────────────────────
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

            // ── Billing background jobs ───────────────────────────────────
            services.AddScoped<MarkOverdueInvoicesJob>();
            services.AddScoped<InvoiceExpiryNotificationJob>();

            // ── Distributed cache (IDistributedCache) ─────────────────────
            services.AddStackExchangeRedisCache(options =>
                options.Configuration =
                    configuration.GetConnectionString("RedisConnection"));

            return services;
        }

        public static IServiceCollection ValidateJwt(
            this IServiceCollection services, IConfiguration configuration)
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
