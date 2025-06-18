namespace Webgostar.Framework.Base.BaseModels
{
    public class SystemError : AggregateRoot
    {
        public string? Data { get; set; }
        public string PersianDate { get; set; }
        public string Message { get; set; }
        public string? InnerExceptionMessage { get; set; }
        public string ErrorFile { get; set; }
    }
}
