using Banking.FanFinancing.Shared.Entities;
using Banking.FanFinancing.Shared.Models;

namespace Banking.FanFinancing.Shared.Repository.Interfaces
{
    public interface ISharedRepository
    {
        Task<(PmdResponseEntity? ModelData, int? StatusCode, string? CompleteResponse, bool? IsSuccessStatusCode)> Pmd(PmdRequestEntity apiRequest, string source, HttpContextApiData httpContext);

        Task<(NadraResponseEntity? ModelData, int? StatusCode, string? CompleteResponse, bool? IsSuccessStatusCode)> Nadra(NadraRequestEntity apiRequest, string source, HttpContextApiData httpContext);

        Task<(SmsResponseEntity? ModelData, int? StatusCode, string? CompleteResponse, bool? IsSuccessStatusCode)> Sms(SmsRequestEntity apiRequest, string source, HttpContextApiData httpContext);


    }
}
