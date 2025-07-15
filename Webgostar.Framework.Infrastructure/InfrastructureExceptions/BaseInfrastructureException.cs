using Webgostar.Framework.Base.BaseExceptions;

namespace Webgostar.Framework.Infrastructure.InfrastructureExceptions;

public abstract class BaseInfrastructureException(string message) : BaseWebGostarException(message);