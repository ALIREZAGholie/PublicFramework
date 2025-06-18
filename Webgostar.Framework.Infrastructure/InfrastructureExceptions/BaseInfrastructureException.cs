using Webgostar.Framework.Base.BaseExceptions;

namespace Webgostar.Framework.Infrastructure.InfrastructureExceptions;

public abstract class BaseInfrastructureException : BaseWebGostarException
{
    protected BaseInfrastructureException(string message) : base(message)
    {

    }
}