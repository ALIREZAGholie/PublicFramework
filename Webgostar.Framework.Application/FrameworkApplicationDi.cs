using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Webgostar.Framework.Application.QueryCommandTools;

namespace Webgostar.Framework.Application
{
    public static class FrameworkApplicationDi
    {
        public static IServiceCollection AddFrameworkApplication(this IServiceCollection services)
        {
            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandValidationBehavior<,>));
            #region MediatR
            services.AddMediatR(cfg =>
           {
               cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(CommandValidationBehavior<,>), ServiceLifetime.Scoped);

               cfg.RegisterServicesFromAssembly(typeof(CommandValidationBehavior<,>).Assembly);
           });

            services.AddScoped(typeof(IPipelineBehavior<,>), typeof(CommandValidationBehavior<,>));

            services.AddValidatorsFromAssembly(typeof(CommandValidationBehavior<,>).Assembly);
            #endregion

            return services;
        }
    }
}