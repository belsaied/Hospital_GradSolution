using Hospital_Grad.API.Extensions;

var builder = WebApplication.CreateBuilder(args);
// Add WebApiServices
builder.Services.AddWebApiServices();

// Add infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddOpenApi();

// Add core services
builder.Services.AddCoreServices();

var app = builder.Build();
// Seed the database with WebApplication extension method
await app.SeedDatabaseAsync();
// Use global exception handling middleware
app.UseExceptionHandlingMiddlewares();


if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerMiddlewares();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();
app.Run();
