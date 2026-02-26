using Hospital_Grad.API.Factories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Hospital_Grad.API.Extensions
{
    public static class WebApiServiceExtensions
    {
        public static IServiceCollection AddWebApiServices(this IServiceCollection services)
        {
            // Add any Web API specific services here (e.g., controllers, filters, etc.)
            services.AddControllers()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(
                            new System.Text.Json.Serialization.JsonStringEnumConverter()
                        );
                    });
            services.AddSignalR();
            services.AddEndpointsApiExplorer();
            services.AddOpenApi();
            services.AddSwaggerGen(options =>
            {
                options.UseInlineDefinitionsForEnums();
            });
            //  CORS policies for development 
            services.AddCors(options =>
            {
                options.AddPolicy("DevPolicy", policy =>
                {
                    policy.WithOrigins()
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();         // Required for SignalR WebSocket upgrade
                });
            });
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ApiResponseFactory.GenerateApiValidationResponse;
            });
            return services;
        }
    }
}