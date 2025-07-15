using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Net;
using Webgostar.Framework.Base.BaseExceptions;
using Webgostar.Framework.Base.IBaseServices;
using Webgostar.Framework.Presentation.Web.ControllerTools;

namespace Webgostar.Framework.Presentation.Web.Middlewares
{

    public static class ApiExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseApiExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiExceptionHandlerMiddleware>();
        }
    }

    public class ApiExceptionHandlerMiddleware(RequestDelegate next)
    {
        public async Task Invoke(HttpContext context, IErrorLogger errorLogger)
        {
            string? message = null;
            AppStatusCode apiStatusCode;

            try
            {
                await next(context);
            }
            catch (BaseWebGostarException exception)
            {
                apiStatusCode = AppStatusCode.LogicError;
                SetErrorMessage(exception);
                await WriteToResponseAsync();
            }
            catch (OptionsValidationException exception)
            {
                apiStatusCode = AppStatusCode.InvalidData;
                SetErrorMessage(exception);
                await WriteToResponseAsync();
            }
            catch (Exception exception)
            {
                await errorLogger.LogError(exception);
                apiStatusCode = AppStatusCode.ServerError;
                SetErrorMessage(new Exception("خطای ناشناخته در سیستم رخ داده است."));
                await WriteToResponseAsync();
            }

            return;

            void SetErrorMessage(Exception exception)
            {
                message = exception.Message;
            }

            async Task WriteToResponseAsync()
            {
                if (context.Response.HasStarted)
                {
                    throw new InvalidOperationException("پاسخ از قبل شروع شده است، میان افزار کد وضعیت درخواست اجرا نخواهد شد.");
                }

                ApiResult result = new()
                {
                    IsSuccess = false,
                    MetaData = new MetaData
                    {
                        AppStatusCode = apiStatusCode,
                        Message = message
                    }
                };

                var json = JsonConvert.SerializeObject(result);

                context.Response.StatusCode = (int)GetStatusCode(apiStatusCode);
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync(json);
            }
        }

        private HttpStatusCode GetStatusCode(AppStatusCode apiStatusCode) =>
            apiStatusCode switch
            {
                AppStatusCode.Success => HttpStatusCode.OK,
                AppStatusCode.NotFound => HttpStatusCode.NotFound,
                AppStatusCode.BadRequest => HttpStatusCode.BadRequest,
                AppStatusCode.LogicError => HttpStatusCode.BadRequest,
                AppStatusCode.UnAuthorize => HttpStatusCode.Unauthorized,
                AppStatusCode.ServerError => HttpStatusCode.InternalServerError,
                AppStatusCode.InvalidData => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };
    }
}
