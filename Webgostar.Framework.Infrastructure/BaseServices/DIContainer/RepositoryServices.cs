using Webgostar.Framework.Base.IBaseServices;
using Webgostar.Framework.Infrastructure.BaseServices.IDIContainer;
using Webgostar.Framework.Infrastructure.InfrastructureIServices;

namespace Webgostar.Framework.Infrastructure.BaseServices.DIContainer
{
    public class RepositoryServices(IAuthService authService, ILoggingContext loggingContext) : IRepositoryServices
    {
        public IAuthService AuthService { get; } = authService;
        public ILoggingContext LoggingContext { get; } = loggingContext;
    }
}
