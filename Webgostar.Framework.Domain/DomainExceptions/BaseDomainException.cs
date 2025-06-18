using Webgostar.Framework.Base.BaseExceptions;

namespace Webgostar.Framework.Domain.DomainExceptions;

public abstract class BaseDomainException : BaseWebGostarException
{
    protected BaseDomainException(string message) : base(message)
    {
    }
}
