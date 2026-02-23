using Domain.Contracts;
using Hospital_Grad.API.MiddleWares;

namespace Hospital_Grad.API.Extensions
{
    public static class WebApplicationExtensions
    {
        public static async Task<WebApplication> SeedDatabaseAsync(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var ObjOfdataSeeding = scope.ServiceProvider.GetRequiredService<IDataSeeding>();
            await ObjOfdataSeeding.SeedDataAsync();
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
    }
}
