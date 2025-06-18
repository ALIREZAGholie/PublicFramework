namespace Webgostar.Framework.Base.BaseExceptions
{
    public abstract class BaseWebGostarException : Exception
    {
        protected BaseWebGostarException(string message) : base(message)
        {
        }
    }
}
