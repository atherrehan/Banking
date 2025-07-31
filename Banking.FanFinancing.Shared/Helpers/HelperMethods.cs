using Banking.FanFinancing.Shared.Exceptions;
using Banking.FanFinancing.Shared.Models;
using Microsoft.AspNetCore.Http;

namespace Banking.FanFinancing.Shared.Helpers
{
    public static class HelperMethods
    {

        public static bool ValidateHeaders(HttpContext _httpContext, bool _checkSession = false)
        {
            bool response = true;
            var headers = _httpContext.Request?.Headers;

            if (headers == null)
            {
                TraceLogger.Log("NullModelException",_httpContext);
                throw new NullModelException();
            }
            else if (string.IsNullOrEmpty(_httpContext?.Request?.Headers["UserID"].FirstOrDefault()))
            {
                TraceLogger.Log("UserID", _httpContext);
                response = false;
            }
            else if (string.IsNullOrEmpty(_httpContext?.Request?.Headers["Password"].FirstOrDefault()))
            {
                TraceLogger.Log("Password", _httpContext);
                response = false;
            }
            else if (string.IsNullOrEmpty(_httpContext?.Request?.Headers["ChannelID"].FirstOrDefault()))
            {
                TraceLogger.Log("ChannelID", _httpContext);
                response = false;
            }
            else
            {
                var channel = _httpContext?.Request?.Headers["ChannelID"].FirstOrDefault()?.ToString().ToLower();
                if (string.IsNullOrEmpty(channel) || !channel.Equals("al baraka bank"))
                {
                    TraceLogger.Log("ChannelID", _httpContext);
                    response = false;
                }
            }
            

            return response;
        }

        public static string GetHeaderValue(HttpContext _httpContext, string _name)
        {
            return _httpContext?.Request.Headers[_name].FirstOrDefault() ?? "";
        }

        public static HttpContextApiData GetHttpContextData(this HttpContext httpContext)
        {
            var User = httpContext?.User?.Identity is CustomIdentity customIdentity && customIdentity.IsAuthenticated
        ? customIdentity
        : null;
            HttpContextApiData apiData = new HttpContextApiData();
            apiData.SourceURL = httpContext?.Request.Path.ToString() ?? "";
            apiData.GUID = httpContext?.Request.Headers["X-GUID"].FirstOrDefault() ?? "0";
            apiData.IPAddress = httpContext?.Connection.RemoteIpAddress?.ToString() ?? "0:0:0:0";
            apiData.LoginGuid = User?.JourneyId ?? "";
            apiData.Endpoint = httpContext?.Request.Host.Value ?? "";
            return apiData;
        }
    }
}
