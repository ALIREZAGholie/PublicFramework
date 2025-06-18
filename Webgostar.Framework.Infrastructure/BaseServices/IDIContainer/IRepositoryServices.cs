using Webgostar.Framework.Base.IBaseServices;
using Webgostar.Framework.Infrastructure.InfrastructureIServices;

namespace Webgostar.Framework.Infrastructure.BaseServices.IDIContainer
{
    public interface IRepositoryServices
    {
        IAuthService AuthService { get; } 
        ILoggingContext LoggingContext { get; }
    }
}
