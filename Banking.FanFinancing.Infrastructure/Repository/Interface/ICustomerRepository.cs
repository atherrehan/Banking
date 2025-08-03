using Banking.FanFinancing.Infrastructure.Entities.Customer;
using Banking.FanFinancing.Shared.Models;

namespace Banking.FanFinancing.Infrastructure.Repository.Interface
{
    public interface ICustomerRepository
    {
        Task<(DataCheckResponseEntity? ModelData, int? StatusCode, string? CompleteResponse, bool? IsSuccessStatusCode)> DataCheck(DataCheckRequestEntity apiRequest, string source, HttpContextApiData httpContext);

        Task<(KiborResponseEntity? ModelData, int? StatusCode, string? CompleteResponse, bool? IsSuccessStatusCode)> Kibor(string source, HttpContextApiData httpContext);

        Task<(ScreeningResponseEntity? ModelData, int? StatusCode, string? CompleteResponse, bool? IsSuccessStatusCode)> Screening(ScreeningRequestEntity apiRequest, string source, HttpContextApiData httpContext);

    }
}
