using Hospital_Grad.API.Extensions;
using Presentation.Hubs;

var builder = WebApplication.CreateBuilder(args);
// Add WebApiServices
builder.Services.AddWebApiServices();

// Add infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Add core services
builder.Services.AddCoreServices();

var app = builder.Build();
// Seed the database with WebApplication extension method
await app.SeedDatabaseAsync();
// Use global exception handling middleware
app.UseExceptionHandlingMiddlewares();


if (app.Environment.IsDevelopment())
{
    app.UseSwaggerMiddlewares();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("NgrokPolicy");
app.UseAuthorization();
app.UseWebSockets();
app.MapControllers();
app.MapHub<AppointmentHub>("/hubs/appointments");
app.Run();
