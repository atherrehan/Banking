using Banking.FanFinancing.Shared.Models;
using Banking.FanFinancing.Shared.Services.Interface;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;
using System.Data;
using Banking.FanFinancing.Shared.DbContext;

namespace Banking.FanFinancing.Shared.Services
{
    public class LoggerService : ILoggerService
    {
        public readonly string ipAddress;
        private readonly IGuidService _GUID;
        private readonly IDbContext _dbContext;
        public LoggerService(IHttpContextAccessor httpContextAccessor, IGuidService gUID, IDbContext dbContext)
        {
            ipAddress = httpContextAccessor.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "";
            _GUID = gUID;
            _dbContext = dbContext;
        }
        public async Task APIError(string CurrentSource, string Error, string StackTrace)
        {
            var log = Log.Logger
               .ForContext("TraceID", "")
               .ForContext("ErrorTime", DateTime.Now)
               .ForContext("RequestID", "")
               .ForContext("Source", CurrentSource)
           .ForContext("ErrorDetails", Error)
               .ForContext("ErrorStackTrace", StackTrace)
               .ForContext("ExceptionType", "APIError");
            await Task.Run(() => { log.Error($"{CurrentSource} |  | {Error}"); });
        }

        public async Task LogException(object RequestResponse, string ControllerPath, Exception ex)
        {
            var source = new StackTrace(ex)?.GetFrame(0)?.GetMethod();
            var log = Log.Logger.ForContext("RequestResponse", JsonConvert.SerializeObject(RequestResponse)).ForContext("TraceID", "").ForContext("URL", ControllerPath)
                .ForContext("IP", ipAddress)
                .ForContext("RequestID", "")
                .ForContext("Source", ControllerPath)
                .ForContext("ErrorPath", $"{source?.DeclaringType?.FullName}/{source?.Name}")
                .ForContext("ErrorMessage", ex.Message)
                .ForContext("ErrorLine", new StackTrace(ex, true).GetFrame(0))
                .ForContext("InnerException", Convert.ToString(ex?.InnerException))
            .ForContext("ErrorDetails", ex?.ToString())
                .ForContext("ExceptionType", "Exception");
            await Task.Run(() => { log.Error($"{ControllerPath} | | {ex?.Message}", Convert.ToString(ex?.InnerException), ex?.ToString()); });
        }

        public async Task LogHttpClientAPI(LogApiRequestResponse log)
        {
            await _dbContext.ExecuteAsync("usp_http_client", new
            {
                Id = log.ExternalId,
                ProcessingCode = log.ProcessingCode,
                Request = log.Request,
                Response = log.Response,
                HttpStatus = log.StatusCode,
                ResponseCode = log.ResponseCode,
                ResponseMessage = log.ResponseMessage,
                RequestDateTime = log.RequestDateTime,
                ResponseDateTime = log.ResponseDateTime,
                Source = log.Source,
                Url = log.Url,
                Guid = log.Guid,
                JourneyId = log.JourneyId,
                PublicIp = log.PublicIp
            }, commandType: CommandType.StoredProcedure);
        }

        public async Task LogRequestResponse(object RequestResponse, string ControllerPath, string Guid = "", string Type = "")
        {
            var log = Log.Logger
              .ForContext("RequestResponse", JsonConvert.SerializeObject(RequestResponse))
              .ForContext("TraceID", Guid)
              .ForContext("URL", ControllerPath)
              .ForContext("IP", ipAddress)
              .ForContext("Type", Type)
              .ForContext("InfoType", "RequestResponse");
            await Task.Run(() => { log.Information($"\n{Type}"); });
        }

        public async Task Exception(object RequestResponse, string ControllerPath, Exception ex)
        {

            var source = new StackTrace(ex)?.GetFrame(0)?.GetMethod();
            var log = Log.Logger.ForContext("RequestResponse", JsonConvert.SerializeObject(RequestResponse)).ForContext("TraceID", _GUID.GUID()).ForContext("URL", ControllerPath)
                .ForContext("IP", ipAddress)
                .ForContext("RequestID", "")
                .ForContext("Source", ControllerPath)
                .ForContext("ErrorPath", $"{source?.DeclaringType?.FullName}/{source?.Name}")
                .ForContext("ErrorMessage", ex.Message)
                .ForContext("ErrorLine", new StackTrace(ex, true).GetFrame(0))
                .ForContext("InnerException", Convert.ToString(ex?.InnerException))
            .ForContext("ErrorDetails", ex?.ToString())
                .ForContext("ExceptionType", "Exception");
            await Task.Run(() => { log.Error($"{ControllerPath} | | {ex?.Message}", Convert.ToString(ex?.InnerException), ex?.ToString()); });
        }

        public async Task LogEndpoint(LogEndpointRequestResponse log)
        {
            await _dbContext.ExecuteAsync("usp_endpoint_log", new
            {
                ProcessingCode = log.ProcessingCode,
                Request = log.Request,
                Response = log.Response,
                ResponseCode = log.ResponseCode,
                ResponseMessage = log.ResponseDescription,
            }, commandType: CommandType.StoredProcedure);
        }
    }
}
