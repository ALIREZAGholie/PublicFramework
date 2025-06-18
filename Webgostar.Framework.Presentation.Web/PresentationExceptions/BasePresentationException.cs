using Webgostar.Framework.Base.BaseExceptions;

namespace Webgostar.Framework.Presentation.Web.PresentationExceptions;

public abstract class BasePresentationException : BaseWebGostarException
{
    protected BasePresentationException(string message) : base(message)
    {

    }
}

