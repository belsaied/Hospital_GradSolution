using Hospital_Grad.API.Factories;
using Microsoft.AspNetCore.Mvc;

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

            services.AddEndpointsApiExplorer();
            services.AddOpenApi();
            services.AddSwaggerGen(options =>
            {
                options.UseInlineDefinitionsForEnums();
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = ApiResponseFactory.GenerateApiValidationResponse;
            });
            return services;
        }
    }
}