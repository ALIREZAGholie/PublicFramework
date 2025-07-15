namespace Webgostar.Framework.Base.BaseModels
{
    public class SystemError : AggregateRoot
    {
        public string? Data { get;  set; }
        public string PersianDate { get; set; }
        public string Message { get;  set; }
        public string? InnerExceptionMessage { get; set; }
        public string ErrorFile { get; set; }

        public SystemError()
        {
            
        }

        public SystemError(string message)
        {
            Message = message;
        }


        public SystemError Edit(string message)
        {
            Message = message;

            return this;
        }
    }
}
