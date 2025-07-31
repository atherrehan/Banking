using Banking.FanFinancing.Shared.Exceptions;
using Banking.FanFinancing.Shared.Models;
using Banking.FanFinancing.Shared.Services.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using ValidationException = Banking.FanFinancing.Shared.Exceptions.ValidationException;

namespace Banking.FanFinancing.Shared.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHostEnvironment _env;

        public ExceptionMiddleware(RequestDelegate next, IHostEnvironment env)
        {
            _next = next;
            _env = env;
        }
        public async Task Invoke(HttpContext httpContext, ILoggerService loggers, IGuidService GUID)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex, _env, loggers, GUID);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex, IHostEnvironment _env, ILoggerService loggers, IGuidService GUID)
        {
            ApiResponse/*<List<string>>*/ errors = new();
            //{
            //    traceId = GUID.GUID()
            //};
            int httpStatusCode;
            switch (ex)
            {
                case CustomException e:
                    httpStatusCode = 200;
                    errors.responseMessage = $"{e.Message}";
                    errors.responseCode = e.StatusCode;
                    //errors.body = new List<string> { e.Message };
                    break;
                case ValidationException e:
                    httpStatusCode = 200;
                    errors.responseMessage = $"{e.Message} - ({e.StatusCode})";
                    errors.responseCode = e.StatusCode;
                    //errors.body = e.modelState;
                    break;
                case NullModelException e:
                    httpStatusCode = 200;
                    errors.responseMessage = $"{e.Message} - (W101)";
                    errors.responseCode = "W101";
                    //errors.body = new List<string> { e.Message };
                    break;
                case BadRequestException e:
                    httpStatusCode = 200;
                    errors.responseMessage = $"{e.Message} - ({e.StatusCode})";
                    errors.responseCode = e.StatusCode;
                    //errors.body = e.Body ?? new List<string> { "Invalid Request" };
                    break;
                case UnAuthorizeException e:
                    httpStatusCode = 200;
                    errors.responseMessage = e.Message;
                    errors.responseCode = "03";
                    //errors.body = new List<string> { e.Message };
                    break;
                case InvalidTokenException e:
                    httpStatusCode = 200;
                    errors.responseMessage = e.Message;
                    errors.responseCode = "W401";
                    //errors.body = new List<string> { e.Message };
                    break;
                default:
                    httpStatusCode = 500;
                    errors.responseCode = "W500";
                    errors.responseMessage = "";
                    errors.responseMessage = ex.Message ?? "Something went wrong, please try again.";
                    if (_env.IsDevelopment())
                    {
                        //errors.body = new List<string> { ex.ToString(), ex.Message ?? "", ex.StackTrace ?? "--", ex.InnerException?.ToString() ?? "--" };
                    }
                    await loggers.Exception(httpContext.Request.Path, "", ex);
                    break;
            }
            httpContext.Response.ContentType = "application/json; charset=utf-8";


            var result = JsonConvert.SerializeObject(new ExceptionResponse/*<object>*/
            {
                responseCode = errors.responseCode,
                responseMessage = errors.responseMessage
            });
            httpContext.Response.StatusCode = httpStatusCode;
            await httpContext.Response.WriteAsync(result);

        }
    }
}
