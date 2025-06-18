using Webgostar.Framework.Base.IBaseServices;
using Webgostar.Framework.Infrastructure.BaseServices.IDIContainer;
using Webgostar.Framework.Infrastructure.InfrastructureIServices;

namespace Webgostar.Framework.Infrastructure.BaseServices.DIContainer
{
    public class RepositoryServices : IRepositoryServices
    {
        public IAuthService AuthService { get; }
        public ILoggingContext LoggingContext { get; }

        public RepositoryServices(IAuthService authService, ILoggingContext loggingContext)
        {
            AuthService = authService;
            LoggingContext = loggingContext;
        }
    }
}
