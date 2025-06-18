namespace Webgostar.Framework.Base.IBaseServices
{
    public interface IErrorLogger
    {
        Task<bool> LogError(Exception error, object? Data = null);
        string GetErrorFilePath(DateTime dateTime);
    }
}
