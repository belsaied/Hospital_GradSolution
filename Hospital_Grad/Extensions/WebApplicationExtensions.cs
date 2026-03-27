using Domain.Contracts;
using Hangfire;
using Hospital_Grad.API.MiddleWares;
using Persistence.Data.Identity;
using Services.Implementations.BillingModule;

namespace Hospital_Grad.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var ObjOfdataSeeding = scope.ServiceProvider.GetRequiredService<IDataSeeding>();
            await ObjOfdataSeeding.SeedDataAsync();
            var identitySeeding = scope.ServiceProvider.GetRequiredService<IdentityDataSeeding>();
            await identitySeeding.SeedAsync();
            return app;
        }
        public static WebApplication UseExceptionHandlingMiddlewares(this WebApplication app)
        {
            app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
            return app;
        }
        public static WebApplication UseSwaggerMiddlewares(this WebApplication app)
        {
            app.MapOpenApi();
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }
        public static WebApplication RegisterBillingRecurringJobs(this WebApplication app)
        {
            RecurringJob.AddOrUpdate<MarkOverdueInvoicesJob>(
                recurringJobId: "billing-mark-overdue",
                methodCall: j => j.ExecuteAsync(),
                cronExpression: "5 0 * * *",
                options: new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

            RecurringJob.AddOrUpdate<InvoiceExpiryNotificationJob>(
                recurringJobId: "billing-expiry-reminders",
                methodCall: j => j.ExecuteAsync(),
                cronExpression: "0 8 * * *",
                options: new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });

            return app;
        }
    }
}
