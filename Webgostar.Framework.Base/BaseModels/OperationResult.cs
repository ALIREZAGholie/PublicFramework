namespace Webgostar.Framework.Base.BaseModels
{
    public class OperationResult<TData>
    {
        public const string SuccessMessage = "عملیات با موفقیت انجام شد";
        public const string ErrorMessage = "خطایی در انجام عملیات رخ داده است";
        public const string NotFoundMessage = "اطلاعات یافت نشد";

        public string Message { get; set; } = "";
        public string? Title { get; set; } = null;
        public OperationResultStatus Status { get; set; }
        public TData? Data { get; set; }


        public static OperationResult<TData> Success(TData data, string message = SuccessMessage)
        {
            return new OperationResult<TData>()
            {
                Status = OperationResultStatus.Success,
                Message = message,
                Data = data,
            };
        }

        public static OperationResult<TData?> NotFound(string message = NotFoundMessage)
        {
            return new OperationResult<TData?>()
            {
                Status = OperationResultStatus.NotFound,
                Message = message,
                Data = default
            };
        }

        public static OperationResult<TData?> Error(string message = ErrorMessage)
        {
            return new OperationResult<TData?>()
            {
                Status = OperationResultStatus.Error,
                Message = message,
                Data = default
            };
        }
    }

    public class OperationResult
    {
        public const string SuccessMessage = "عملیات با موفقیت انجام شد";
        public const string ErrorMessage = "خطایی در انجام عملیات رخ داده است";
        public const string NotFoundMessage = "اطلاعات یافت نشد";

        public string Message { get; set; }
        public string Title { get; set; } = null;
        public OperationResultStatus Status { get; set; }

        public static OperationResult Success(string message = SuccessMessage)
        {
            return new OperationResult()
            {
                Status = OperationResultStatus.Success,
                Message = message,
            };
        }

        public static OperationResult NotFound(string message = NotFoundMessage)
        {
            return new OperationResult()
            {
                Status = OperationResultStatus.NotFound,
                Message = message,
            };
        }

        public static OperationResult Error(string message = ErrorMessage)
        {
            return new OperationResult()
            {
                Status = OperationResultStatus.Error,
                Message = message,
            };
        }
    }

    public enum OperationResultStatus
    {
        Success = 200,
        Error = 400,
        NotFound = 404
    }
}