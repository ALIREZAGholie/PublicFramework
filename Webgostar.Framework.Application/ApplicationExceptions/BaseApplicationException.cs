using Webgostar.Framework.Base.BaseExceptions;

namespace Webgostar.Framework.Application.ApplicationExceptions;

public abstract class BaseApplicationException : BaseWebGostarException
{
    protected BaseApplicationException(string message) : base(message)
    {

    }
}

