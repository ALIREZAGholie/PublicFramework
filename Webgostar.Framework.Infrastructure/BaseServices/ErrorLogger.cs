using System.Dynamic;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using Webgostar.Framework.Base.BaseExtensions;
using Webgostar.Framework.Base.BaseModels;
using Webgostar.Framework.Base.IBaseServices;

namespace Webgostar.Framework.Infrastructure.BaseServices
{
    public class ErrorLogger : IErrorLogger
    {
        private readonly IWebHostEnvironment _environment;

        public ErrorLogger(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        async Task<bool> IErrorLogger.LogError(Exception error, object? data)
        {
            try
            {
                var dateTime = DateTime.Now;

                var path = GetErrorFilePath(dateTime);

                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                var dateTimeStr = dateTime.ToString("yyyy.MM.dd.HH.mm.ss");

                var fileName = $"{dateTimeStr}--{Guid.NewGuid()}.txt";
                dynamic errorObject = new ExpandoObject();

                errorObject.Exception = error;

                SystemError systemError = new();

                if (data != null)
                {
                    errorObject.Data = JsonConvert.SerializeObject(data);
                    systemError.Data = JsonConvert.SerializeObject(data);
                }

                errorObject.CreateDate = dateTime;
                errorObject.PersianDate = dateTime.ToShamsiWhitTime();

                systemError.PersianDate = dateTime.ToShamsiWhitTime();

                systemError.Message = error.Message + " ### " + error.StackTrace;
                systemError.InnerExceptionMessage = error.InnerException?.Message;

                systemError.ErrorFile = fileName;
                await File.WriteAllLinesAsync(Path.Combine(path, fileName),
                    new List<string> { JsonConvert.SerializeObject(errorObject) });

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public string GetErrorFilePath(DateTime dateTime)
        {
            try
            {
                var path = _environment.ContentRootPath;

                path = Path.Combine(path, "Errors", $"{dateTime.Year.ToString()}.{dateTime.Month.ToString()}.{dateTime.Day.ToString()}");

                return path;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
