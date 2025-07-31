using Banking.FanFinancing.Shared.DbContext;
using Banking.FanFinancing.Shared.Entities;
using Banking.FanFinancing.Shared.Enums;
using Banking.FanFinancing.Shared.HttpClient.Interface;
using Banking.FanFinancing.Shared.Models;
using Banking.FanFinancing.Shared.Repositry.Interfaces;

namespace Banking.FanFinancing.Shared.Repository
{
    public class SharedRepository : ISharedRepository
    {
        private readonly IDbContext _dbContext;
        private readonly IHTTPClientFactoryService _hTTPClient;
        public SharedRepository(IDbContext dbContext, IHTTPClientFactoryService hTTPClient)
        {
            _dbContext = dbContext;
            _hTTPClient = hTTPClient;
        }

        public async Task<(NadraResponseEntity? ModelData, int? StatusCode, string? CompleteResponse, bool? IsSuccessStatusCode)> Nadra(NadraRequestEntity apiRequest, string source, HttpContextApiData httpContext)
        {
            ApiUrl? url = await _dbContext.GetURL("Nadra");
            if (url.URL.Equals("url_nadra") || httpContext.Endpoint.Equals("localhost:5501") || httpContext.Endpoint.Equals("139.59.88.149:5501"))
            {
                var response = new NadraResponseEntity();
                response.status = new() { code = 200, message = "Success" };
                response.data = new();
                response.data.responseData = new() { citizenNumber = apiRequest.cnic };
                response.data.responseData.responseStatus = new() { CODE = "100", MESSAGE = "Success" };
                response.data.responseData.personData = new() { birthPlace = "لاہور,لاہور", dateOfBirth = "07-12-1999", expiryDate = "2028-07-16", fatherHusbandName = "طارق محمود", motherName = "روبینہ کوثر", name = "محمد اسامہ", permanentAddress = "مکان نمبر 5،گلی نمبر 12،محلہ مصطفےٰ آباد دھرم پورہ،لاہور کینٹ،ضلع لاہور", photograph = "", presentAddress = "مکان نمبر A-70،محلہ علامہ اقبال ٹاؤن،بلاک پاک،لاہور" };
                return (response, 200, "", true);
            }
            var ApiResponse = await _hTTPClient
               .PostAsync<NadraResponseEntity>(
               RequestType.REST,
               httpContext,
               $"{source}/SharedRepository/Nadra",
               url,
               apiRequest,
               null,
               null);

            return (ApiResponse.IsSuccessStatusCode == true ? ApiResponse.ModelData : default,
                ApiResponse.StatusCode, ApiResponse.CompleteResponse, ApiResponse.IsSuccessStatusCode);

            //ApiUrl? url = await _dbContext.GetURL("Nadra");
            //url.Soap = url.Soap.Replace("{franchizeID}", apiRequest.FranchiseId);
            //url.Soap = url.Soap.Replace("{username}", url.UserName);
            //url.Soap = url.Soap.Replace("{password}", url.Password);
            //url.Soap = url.Soap.Replace("{transactionid}", apiRequest.TransactionId);
            //url.Soap = url.Soap.Replace("{citizenNumber}", apiRequest.CitizenNumber);
            //url.Soap = url.Soap.Replace("{issuedate}", apiRequest.IssueDate);
            //url.Soap = url.Soap.Replace("{birthyear}", apiRequest.BirthYear);
            //url.Soap = url.Soap.Replace("{areaName}", apiRequest.AreaName);
            //if (url.URL.Equals("url_nadra"))
            //{
            //    var response = new NadraResponseEntity();
            //    response.RESPONSESTATUS = new() { CODE = "100", MESSAGE = "Success" };
            //    response.PERSONALDATA = new PERSONALDATA()
            //    {
            //        BIRTHPLACE = "Pluto",
            //        DATEOFBIRTH = "1986-10-10",
            //        EXPIRYDATE = "2026-06-09",
            //        FATHERHUSBANDNAME = "Doe John",
            //        MOTHERNAME = "Jullie",
            //        NAME = "John Doe",
            //        PERMANANTADDRESS = "Mars",
            //        PHOTOGRAPH = "N/A",
            //        PRESENTADDRESS = "Earth"
            //    };
            //    response.CARDTYPE = string.Empty;
            //    response.CITIZENNUMBER = apiRequest.CitizenNumber;
            //    return (response, 200, "", true);
            //}
            //var ApiResponse = await _hTTPClient
            //   .PostAsync<NadraRootEntity>(
            //   RequestType.SOAP,
            //   httpContext,
            //   $"{source}/CustomerRepository/Nadra",
            //   url,
            //   url.Soap,
            //   null,
            //   null);

            //return (ApiResponse.IsSuccessStatusCode == true ? ApiResponse.ModelData?.RESPONSEDATA : default,
            //    ApiResponse.StatusCode, ApiResponse.CompleteResponse, ApiResponse.IsSuccessStatusCode);
        }

        public async Task<(PmdResponseEntity? ModelData, int? StatusCode, string? CompleteResponse, bool? IsSuccessStatusCode)> Pmd(PmdRequestEntity apiRequest, string source, HttpContextApiData httpContext)
        {
            ApiUrl? url = await _dbContext.GetURL("Pmd");
            url.Soap = url.Soap.Replace("{userName}", url.UserName);
            url.Soap = url.Soap.Replace("{passwd}", url.Password);
            url.Soap = url.Soap.Replace("{cnic}", apiRequest.cnic);
            url.Soap = url.Soap.Replace("{msisdn}", apiRequest.msisdn.Substring(1));
            url.Soap = url.Soap.Replace("{transactionId}", apiRequest.transactionId);
            if (url.URL.Equals("url_pmd") || httpContext.Endpoint.Equals("localhost:5501") || httpContext.Endpoint.Equals("139.59.88.149:5501"))
            {
                return (new PmdResponseEntity() { ax21responseCode = "01", ax21message = "yes", ax21status = "00" }, 200, "", true);//Remove this if block once url are provided

            }
            var ApiResponse = await _hTTPClient
               .PostAsync<PmdRootEntity>(
               RequestType.SOAP,
               httpContext,
               $"{source}/CustomerRepository/PMD",
               url,
               url.Soap,
               null,
               null);

            return (ApiResponse.IsSuccessStatusCode == true ? ApiResponse.ModelData?.soapenvEnvelope?.soapenvBody?.nsverifyResponse?.nsreturn : default,
                ApiResponse.StatusCode, ApiResponse.CompleteResponse, ApiResponse.IsSuccessStatusCode);

        }

        public async Task<(SmsResponseEntity? ModelData, int? StatusCode, string? CompleteResponse, bool? IsSuccessStatusCode)> Sms(SmsRequestEntity apiRequest, string source, HttpContextApiData httpContext)
        {
            ApiUrl? url = await _dbContext.GetURL("Sms");
            url.Soap = url.Soap.Replace("{MobileNo}", apiRequest.MobileNo);
            url.Soap = url.Soap.Replace("{date}", apiRequest.Date);
            url.Soap = url.Soap.Replace("{time}", apiRequest.Time);
            url.Soap = url.Soap.Replace("{tranType}", apiRequest.TranType);
            url.Soap = url.Soap.Replace("{Message}", apiRequest.Message);
            url.Soap = url.Soap.Replace("{TransactionNature}", apiRequest.TransactionNature);
            url.Soap = url.Soap.Replace("{rrn}", apiRequest.Rrn);
            if (url.URL.Equals("url_sms") || httpContext.Endpoint.Equals("localhost:5501") || httpContext.Endpoint.Equals("139.59.88.149:5501"))
            {
                return (new SmsResponseEntity() { Code = "00", Message = "Url not found", Status = "00" }, 200, "", true);//Remove this if block once url are provided

            }
            var ApiResponse = await _hTTPClient
               .PostAsync<SmsRootEntity>(
               RequestType.SOAP,
               httpContext,
               $"{source}/CustomerRepository/Sms",
               url,
               url.Soap,
               null,
               null);

            return (ApiResponse.IsSuccessStatusCode == true ? ApiResponse.ModelData?.Body?.VerifyResponse?.Return : default,
              ApiResponse.StatusCode, ApiResponse.CompleteResponse, ApiResponse.IsSuccessStatusCode);
        }

    }
}
