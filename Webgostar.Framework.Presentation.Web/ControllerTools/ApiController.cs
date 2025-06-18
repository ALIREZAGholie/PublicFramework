using Microsoft.AspNetCore.Mvc;
using System.Net;
using Webgostar.Framework.Base.BaseModels;

namespace Webgostar.Framework.Presentation.Web.ControllerTools;

[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class ApiController : ControllerBase
{
    protected ApiResult CommandResult(OperationResult result)
    {
        return new ApiResult()
        {
            IsSuccess = result.Status == OperationResultStatus.Success,
            MetaData = new MetaData
            {
                Message = result.Message,
                AppStatusCode = result.Status.MapOperationStatus()
            }
        };
    }

    protected ApiResult<TData?> CommandResult<TData>(OperationResult<TData> result, HttpStatusCode statusCode = HttpStatusCode.OK, string locationUrl = null)
    {
        var isSuccess = result.Status == OperationResultStatus.Success;
        if (!isSuccess)
        {
            return new ApiResult<TData?>()
            {
                IsSuccess = isSuccess,
                Data = isSuccess ? result.Data : default,
                MetaData = new MetaData
                {
                    Message = result.Message,
                    AppStatusCode = result.Status.MapOperationStatus()
                }
            };
        }

        HttpContext.Response.StatusCode = (int)statusCode;

        if (!string.IsNullOrWhiteSpace(locationUrl))
        {
            HttpContext?.Response?.Headers?.Add("location", locationUrl);
        }

        return new ApiResult<TData?>()
        {
            IsSuccess = isSuccess,
            Data = isSuccess ? result.Data : default,
            MetaData = new MetaData
            {
                Message = result.Message,
                AppStatusCode = result.Status.MapOperationStatus()
            }
        };
    }

    protected ApiResult<TData> QueryResult<TData>(TData result)
    {
        return new ApiResult<TData>()
        {
            IsSuccess = true,
            Data = result,
            MetaData = new MetaData
            {
                Message = "عملیات با موفقیت انجام شد",
                AppStatusCode = AppStatusCode.Success
            }
        };
    }
}

public static class EnumHelper
{
    public static AppStatusCode MapOperationStatus(this OperationResultStatus status)
    {
        return status switch
        {
            OperationResultStatus.Success => AppStatusCode.Success,
            OperationResultStatus.NotFound => AppStatusCode.NotFound,
            OperationResultStatus.Error => AppStatusCode.LogicError,
            _ => AppStatusCode.LogicError
        };
    }
}