using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Data;
using System.Reflection;
using Webgostar.Framework.Base.BaseModels.DbModels;
using Webgostar.Framework.Base.IBaseServices;
using Webgostar.Framework.Infrastructure.BaseServices;
using Webgostar.Framework.Infrastructure.BaseServices.DIContainer;
using Webgostar.Framework.Infrastructure.BaseServices.IDIContainer;
using Webgostar.Framework.Infrastructure.InfrastructureIServices;
using Webgostar.Framework.Infrastructure.InfrastructureServices;
using Webgostar.Framework.Infrastructure.MediatR;

namespace Webgostar.Framework.Infrastructure;

public static class FrameworkInfrastuctureDi
{
    /// <summary>
    /// Add FrameworkInfrastucture Dependency Injection
    /// </summary>
    /// <param name="services">IServiceCollection</param>
    /// <param name="useCaseAssembly">Assembly where UseCases is located</param>
    /// <param name="queryAssembly">Assembly where Queries is located</param>
    /// <returns>ServiceCollection</returns>
    public static IServiceCollection AddFrameworkInfrastucture(this IServiceCollection services, IConfiguration configuration, Assembly useCaseAssembly, Assembly queryAssembly)
    {
        services.AddHttpContextAccessor();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IErrorLogger, ErrorLogger>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<ICustomPublisher, CustomPublisher>();

        services.AddScoped<IDbConnection>(sp => new SqlConnection(configuration.GetConnectionString("DefaultConnection")));

        services.Configure<MongoDbConfig>(configuration.GetSection("MongoDbConfig"));

        services.AddSingleton<IMongoClient, MongoClient>(sp => new MongoClient(configuration.GetSection("MongoDbConfig:ConnectionString").Value));

        services.AddScoped(sp =>
        {
            var client = sp.GetRequiredService<IMongoClient>();
            var settings = sp.GetRequiredService<IOptions<MongoDbConfig>>().Value;
            return client.GetDatabase(settings.DatabaseName);
        });

        services.AddScoped<ILogService, MongoLogService>();
        services.AddScoped<ILoggingContext, LoggingContext>();

        #region MediatR
        services.AddMediatR(cfg =>
       {
           cfg.RegisterServicesFromAssembly(typeof(CustomPublisher).Assembly);
           cfg.RegisterServicesFromAssembly(useCaseAssembly);
           cfg.RegisterServicesFromAssembly(queryAssembly);
       });

        services.AddValidatorsFromAssembly(typeof(CustomPublisher).Assembly);
        services.AddValidatorsFromAssembly(useCaseAssembly);
        services.AddValidatorsFromAssembly(queryAssembly);
        #endregion

        services.AddScoped<IRepositoryServices, RepositoryServices>();

        return services;
    }
}
