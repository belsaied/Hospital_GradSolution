using Hospital_Grad.API.Factories;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Grad.API.Extensions
{
    public static class WebApiServiceExtensions
    {
        public static IServiceCollection AddWebApiServices(
            this IServiceCollection services)
        {
            services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(
                        new System.Text.Json.Serialization.JsonStringEnumConverter());
                });


            services.AddEndpointsApiExplorer();
            services.AddOpenApi();
            services.AddMemoryCache();

            services.AddSwaggerGen(options =>
            {
                options.UseInlineDefinitionsForEnums();
            });

            services.AddCors(options =>
            {
                options.AddPolicy("DevPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory =
                    ApiResponseFactory.GenerateApiValidationResponse;
            });
            return services;
        }
    }
}