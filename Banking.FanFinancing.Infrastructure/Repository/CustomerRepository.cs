using Banking.FanFinancing.Infrastructure.Entities.Customer;
using Banking.FanFinancing.Infrastructure.Repository.Interface;
using Banking.FanFinancing.Shared.DbContext;
using Banking.FanFinancing.Shared.Enums;
using Banking.FanFinancing.Shared.HttpClient.Interface;
using Banking.FanFinancing.Shared.Models;

namespace Banking.FanFinancing.Infrastructure.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IDbContext _dbContext;
        private readonly IHTTPClientFactoryService _hTTPClient;
        public CustomerRepository(IDbContext dbContext, IHTTPClientFactoryService hTTPClient)
        {
            _dbContext = dbContext;
            _hTTPClient = hTTPClient;
        }

        public async Task<(KiborResponseEntity? ModelData, int? StatusCode, string? CompleteResponse, bool? IsSuccessStatusCode)> Kibor(string source, HttpContextApiData httpContext)
        {
            ApiUrl? url = await _dbContext.GetURL("Kibor");
            if (url.URL.Equals("url_kibor") || httpContext.Endpoint.Equals("localhost:5501") || httpContext.Endpoint.Equals("139.59.88.149:5501"))
            {
                return (new KiborResponseEntity() { status = "200", message = "Url not found", data = new() { rate = "12.37" } }, 200, "", true);//Remove this if block once url are provided

            }
            var ApiResponse = await _hTTPClient
               .PostAsync<KiborResponseEntity>(
               RequestType.GET,
               httpContext,
               $"{source}/CustomerRepository/Kibor",
               url,
               null,
               null,
               null);

            return ApiResponse;
        }    

        public async Task<(DataCheckResponseEntity? ModelData, int? StatusCode, string? CompleteResponse, bool? IsSuccessStatusCode)> DataCheck(DataCheckRequestEntity apiRequest, string source, HttpContextApiData httpContext)
        {
            ApiUrl? url = await _dbContext.GetURL("DataCheck");
            if (url.URL.Equals("url_datacheck") || httpContext.Endpoint.Equals("localhost:5501") || httpContext.Endpoint.Equals("139.59.88.149:5501"))
            {
                return (new DataCheckResponseEntity() { Report = new() { CCP_SUMMARY_TOTAL = [] }, Status = "00" }, 200, "", true);//Remove this if block once url are provided

            }
            var ApiResponse = await _hTTPClient
               .PostAsync<DataCheckResponseEntity>(
               RequestType.REST,
               httpContext,
               $"{source}/CustomerRepository/DataCheck",
               url,
               apiRequest,
               null,
               null);

            return ApiResponse;
        }

        public async Task<(ScreeningResponseEntity? ModelData, int? StatusCode, string? CompleteResponse, bool? IsSuccessStatusCode)> Screening(ScreeningRequestEntity apiRequest, string source, HttpContextApiData httpContext)
        {
            ApiUrl? url = await _dbContext.GetURL("Acl");
            if (url.URL.Equals("url_screening") || httpContext.Endpoint.Equals("localhost:5501") || httpContext.Endpoint.Equals("139.59.88.149:5501"))
            {
                var screeningResponse = new ScreeningResponseEntity
                {
                    responseType = "simple",
                    screeningStatus = "success",
                    importStatus = "unchanged",
                    correlationId = "050620250502",
                    uniqueId = "27052024-Trade",
                    accountId = 1,
                    datasetId = 27,
                    datasetName = "NEECA",
                    caseId = 609965,
                    totalSubCases = 2,
                    totalMatches = 0,
                    subCaseDetails = new List<ScreeningResponseSubCaseDetail>
    {
        new() {
            subcase = "GWL",
            requiresReview = "Y",
            status = "Open",
            assignedUser = "N/A - Unassigned",
            totalMatches = 8
        },
        new() {
            subcase = "PEP/EDD",
            requiresReview = "Y",
            status = "Open",
            assignedUser = "N/A - Unassigned",
            totalMatches = 4
        }
    }
                };
                return (screeningResponse, 200, "", true);
            }
            var ApiResponse = await _hTTPClient
               .PostAsync<ScreeningResponseEntity>(
               RequestType.POST,
               httpContext,
               $"{source}/CustomerRepository/Screening",
               url,
               apiRequest,
               null,
               null);

            return ApiResponse;
        }


    }
}
