using Webgostar.Framework.Base.BaseExceptions;

namespace Webgostar.Framework.Application.ApplicationExceptions;

public abstract class BaseApplicationException(string message) : BaseWebGostarException(message);

