using Hangfire;
using Hospital_Grad.API.Extensions;
using Presentation.Hubs;

var builder = WebApplication.CreateBuilder(args);
// Add WebApiServices
builder.Services.AddWebApiServices();

// Add infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add core services
builder.Services.AddCoreServices(builder.Configuration);

var app = builder.Build();
app.UseHangfireDashboard("/hangfire");
// Seed the database with WebApplication extension method
await app.SeedDatabaseAsync();
// Use global exception handling middleware
app.UseExceptionHandlingMiddlewares();
app.Use(async (ctx, next) =>
{
    ctx.Request.EnableBuffering();
    await next();
});

if (app.Environment.IsDevelopment())
{
    app.UseSwaggerMiddlewares();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("DevPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.RegisterBillingRecurringJobs();
app.UseWebSockets();
app.MapControllers();
app.MapHub<AppointmentHub>("/hubs/appointments");
app.MapHub<WardHub>("/hubs/beds");
app.Run();
