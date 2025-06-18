using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using Webgostar.Framework.Presentation.Web.ControllerTools;
using Webgostar.Framework.Presentation.Web.Utilites;

namespace Webgostar.Framework.Presentation.Web
{
    public static class FrameworkPresentationWebDi
    {
        public static IServiceCollection AddFrameworkPresentationWeb(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddJwtAuthentication(configuration)
                .AddSwagger()
                .AddMapsterConfig();

            services.AddCors(options =>
            {
                options.AddPolicy(name: "TarazWebGostar", builder =>
                {
                    var origins = configuration["App:CorsOrigins"]?
                        .Split(",", StringSplitOptions.RemoveEmptyEntries);
                    builder.WithOrigins(origins)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddMemoryCache();

            services.AddHsts(options =>
            {
                options.Preload = true;
                options.IncludeSubDomains = true;
                options.MaxAge = TimeSpan.FromDays(60);
            });

            services.AddControllers(option => option.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)
                .AddNewtonsoftJson(option => option.SerializerSettings.ContractResolver = new DefaultContractResolver())
                .AddJsonOptions(option => option.JsonSerializerOptions.PropertyNamingPolicy = null)
                .ConfigureApiBehaviorOptions(option =>
                {
                    option.InvalidModelStateResponseFactory = context =>
                        throw new Exception(ModelStateUtilites.GetModelStateErrors(context.ModelState));
                });

            return services;
        }
    }
}