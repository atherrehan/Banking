using Microsoft.Extensions.Configuration;
using Serilog.Events;
using Serilog;

namespace Banking.FanFinancing.Shared.Configs
{
    public static class LogConfigs
    {
        private static string logPath = string.Empty;
        public static void ConfigureSerilog(IConfiguration configuration)
        {
            logPath = configuration.GetSection("LogFilePath").GetSection("RequestResponseLogPath").Value ?? "";
            Log.Logger = ConfigureDebugLevelLogs(ConfigureErrorLevelLogs(ConfigureInformationLevelLogs(LoggerEnrichment(new LoggerConfiguration()))))
                         .WriteTo.Seq("http://localhost:5341")
                         .WriteTo.Console()
                         .CreateLogger();

        }
        private static LoggerConfiguration LoggerEnrichment(LoggerConfiguration loggerConfiguration)
        {

            return loggerConfiguration
               .Enrich.FromLogContext()
               .MinimumLevel.Information()
               .MinimumLevel.Verbose()
               .MinimumLevel.Debug()
               .WriteTo.Console()
               .MinimumLevel.Override("Default", LogEventLevel.Fatal)
               .MinimumLevel.Override("Microsoft", LogEventLevel.Fatal)
               .MinimumLevel.Override("System", LogEventLevel.Fatal)
               .Enrich.WithProperty("RequestResponse", "")
               .Enrich.WithProperty("URL", "")
               .Enrich.WithProperty("TraceID", "")
               .Enrich.WithProperty("RequestTime", "")
               .Enrich.WithProperty("IP", "")
               .Enrich.WithProperty("Type", "")
               .Enrich.WithProperty("LogType", "")
               .Enrich.WithProperty("RequestID", "")
               .Enrich.WithProperty("Source", "")
               .Enrich.WithProperty("ErrorPath", "")
               .Enrich.WithProperty("Error", "")
               .Enrich.WithProperty("ErrorMessage", "")
               .Enrich.WithProperty("ErrorLine", "")
               .Enrich.WithProperty("InnerException", "")
               .Enrich.WithProperty("ErrorDetails", "")
               .Enrich.WithProperty("Data", "")
               .Enrich.WithProperty("ExceptionType", "")
               .Enrich.WithProperty("ErrorTime", "")
               .Enrich.WithProperty("ErrorStackTrace", "");
        }
        private static LoggerConfiguration ConfigureInformationLevelLogs(LoggerConfiguration loggerConfiguration)
        {
            return loggerConfiguration
         .WriteTo.Logger(l => l
             .Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Information)
             .WriteTo.Map("InfoType", string.Empty, (type, tw) =>
             {
                 if (type.Equals("RequestResponse"))
                 {
                     tw.File($"{logPath}/Request_Response.txt",
                         outputTemplate: "{Timestamp:dd/MM/yyyy hh:mm:ss ffff tt} | URI: {URL} | IP: {IP} | GUID: {TraceID} | Type {Type} : {RequestResponse}\n",
                         rollingInterval: RollingInterval.Day,
                         retainedFileCountLimit: null);
                 }
             }));
        }
        private static LoggerConfiguration ConfigureErrorLevelLogs(LoggerConfiguration loggerConfiguration)
        {
            return loggerConfiguration
                .WriteTo.Console(outputTemplate: "{Timestamp:dd/MM/yyyy hh:mm:ss ffff tt} | URI: {URL} | IP: {IP} | " +
                                                   "GUID: {TraceID}  | Type {Type} : {RequestResponse}" +
                                                   " |  Request ID : {RequestID} | Source : {Source} | Error Path : {ErrorPath} | " +
                                                   "Error Message : {ErrorMessage} | Error Line : {ErrorLine} | Inner Exception : {InnerException} | Error Details : {ErrorDetails} \n")
                   .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Error)
                   .WriteTo.Map("ExceptionType", string.Empty, (type, wt) =>
                   {

                       if (type.Equals("Exception"))
                       {
                           wt.File($"{logPath}/Exceptions.txt",
                                   outputTemplate: "{Timestamp:dd/MM/yyyy hh:mm:ss ffff tt} | URI: {URL} | IP: {IP} | " +
                                                   "GUID: {TraceID}  | Type {Type} : {RequestResponse}" +
                                                   " |  Request ID : {RequestID} | Source : {Source} | Error Path : {ErrorPath} | " +
                                                   "Error Message : {ErrorMessage} | Error Line : {ErrorLine} | Inner Exception : {InnerException} | Error Details : {ErrorDetails} \n",
                                   rollingInterval: RollingInterval.Day,
                                   retainedFileCountLimit: null);
                       }
                       else if (type.Equals("APIException"))
                       {
                           wt.File($"{logPath}/APIException.txt",
                                   outputTemplate: "{Timestamp:dd/MM/yyyy hh:mm:ss ffff tt} | " +
                                                    "GUID: {TraceID} " +
                                                    " |  Request ID : {RequestID} | Source : {Source} | " +
                                                    "Error Time : {ErrorTime}  |Error Stack Trace : {ErrorStackTrace} | Error Details : {ErrorDetails} \n",
                                   rollingInterval: RollingInterval.Day,
                                   retainedFileCountLimit: null);
                       }


                   }));
        }
        private static LoggerConfiguration ConfigureDebugLevelLogs(LoggerConfiguration loggerConfiguration)
        {
            return loggerConfiguration
                .WriteTo.Console(outputTemplate: "{Timestamp:dd/MM/yyyy hh:mm:ss ffff tt} | URI: {URL} | GUID: {TraceID} | Request Time : {RequestTime}| Request ID : {RequestID}| Source: {Source}| Data : {Data} |  \n")
                   .WriteTo.Logger(l => l.Filter.ByIncludingOnly(e => e.Level == LogEventLevel.Debug)
                   .WriteTo.Map("DebugType", string.Empty, (type, tw) =>
                   {
                       tw.File($"{logPath}/Debug.txt",
                          outputTemplate: "{Timestamp:dd/MM/yyyy hh:mm:ss ffff tt} | URI: {URL} | GUID: {TraceID} | Request Time : {RequestTime}| Request ID : {RequestID}| Source: {Source}| Data : {Data} |  \n",
                          rollingInterval: RollingInterval.Day,
                          retainedFileCountLimit: null);
                   }));
        }
    }
}
