using Banking.FanFinancing.Shared.Models;

namespace Banking.FanFinancing.Shared.Services.Interface
{
    public interface ILoggerService
    {
        Task LogRequestResponse(object RequestResponse, string ControllerPath, string Guid = "", string Type = "");

        Task LogHttpClientAPI(LogApiRequestResponse log);

        Task LogException(object RequestResponse, string ControllerPath, Exception ex);

        Task APIError(string CurrentSource, string Error, string StackTrace);

        Task Exception(object RequestResponse, string ControllerPath, Exception ex);

        Task LogEndpoint(LogEndpointRequestResponse log);


    }
}
