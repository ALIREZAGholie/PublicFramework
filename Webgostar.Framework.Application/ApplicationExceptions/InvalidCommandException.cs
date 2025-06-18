namespace Webgostar.Framework.Application.ApplicationExceptions;

public class InvalidCommandException : BaseApplicationException
{
    public InvalidCommandException(string message) : base(message)
    {
    }
}

