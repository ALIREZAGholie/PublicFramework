using Webgostar.Framework.Base.BaseExceptions;

namespace Webgostar.Framework.Domain.DomainExceptions;

public abstract class BaseDomainException(string message) : BaseWebGostarException(message);
