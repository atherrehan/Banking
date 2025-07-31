using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System.Runtime.CompilerServices;

namespace Banking.FanFinancing.Shared.Helpers
{
    public static class TraceLogger
    {
        public static void Log(
         string message,
         HttpContext? context = null,
         [CallerMemberName] string memberName = "",
         [CallerFilePath] string filePath = "",
         [CallerLineNumber] int lineNumber = 0)
        {
            var fileName = Path.GetFileName(filePath);

            // Get controller & action name from route
            string controller = context?.GetRouteValue("controller")?.ToString() ?? "UnknownController";
            string action = context?.GetRouteValue("action")?.ToString() ?? "UnknownAction";

            Console.WriteLine(
                $"""
            ========== Trace Log ==========
            Controller : {controller}
            Action     : {action}     
            Caller     : {memberName}            
            Service   : {fileName}:{lineNumber}
            Message    : {message}
            ================================
            """);
        }
    }
}
