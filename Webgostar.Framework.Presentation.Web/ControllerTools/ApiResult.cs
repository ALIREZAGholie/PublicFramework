namespace Webgostar.Framework.Presentation.Web.ControllerTools;

public class ApiResult
{
    public bool IsSuccess { get; set; }
    public MetaData? MetaData { get; set; }
}
public class ApiResult<TData>
{
    public bool IsSuccess { get; set; }
    public TData? Data { get; set; }
    public MetaData? MetaData { get; set; }
}
public class MetaData
{
    public string? Message { get; set; }
    public AppStatusCode AppStatusCode { get; set; }
}

public enum AppStatusCode
{
    Success = 200,
    BadRequest = 400,
    UnAuthorize = 401,
    NotFound = 404,
    ServerError = 500,
    LogicError = 4,
    InvalidData = 7
}