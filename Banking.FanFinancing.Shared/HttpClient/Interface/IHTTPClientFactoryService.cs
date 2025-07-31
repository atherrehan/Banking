using Banking.FanFinancing.Shared.Enums;
using Banking.FanFinancing.Shared.Models;

namespace Banking.FanFinancing.Shared.HttpClient.Interface
{
    public interface IHTTPClientFactoryService
    {
        Task<(T? ModelData, int? StatusCode, string? CompleteResponse, bool? IsSuccessStatusCode)> PostAsync<T>(RequestType _RequestType, HttpContextApiData httpContextData, string source, ApiUrl URL, object param, List<ApiHeaders>? apiHeaders = default, LogKeys? logKeys = default);

    }
}
