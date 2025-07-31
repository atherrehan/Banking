using Banking.FanFinancing.Shared.DbContext;
using Banking.FanFinancing.Shared.Enums;
using Banking.FanFinancing.Shared.HttpClient.Interface;
using Banking.FanFinancing.Shared.Models;
using Banking.FanFinancing.Shared.Services.Interface;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Xml;

namespace Banking.FanFinancing.Shared.HttpClient
{
    public class HTTPClientFactoryService : IHTTPClientFactoryService
    {
        private readonly ILoggerService _loggerService;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly IGuidService _guidService;
        private readonly IDbContext _dbContext;

        public HTTPClientFactoryService(ILoggerService loggerService, IHttpClientFactory httpClientFactory, IBackgroundTaskQueue backgroundTaskQueue, IGuidService guidService, IDbContext dbContext)
        {
            _loggerService = loggerService;
            _httpClientFactory = httpClientFactory;
            _backgroundTaskQueue = backgroundTaskQueue;
            _guidService = guidService;
            _dbContext = dbContext;
        }
        private async Task<string?> GetAccessTokenAsync()
        {
            ApiUrl? url = await _dbContext.GetURL("AclAuth");
            var tokenUrl = url.URL;
            var clientId = url.UserName;
            var clientSecret = url.Password;
            var scope = url.Soap;

            var httpClient = _httpClientFactory.CreateClient(); // Use injected factory

            var request = new HttpRequestMessage(HttpMethod.Post, tokenUrl);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var content = new StringContent($"grant_type=client_credentials&scope={scope}",
                                            Encoding.UTF8,
                                            "application/x-www-form-urlencoded");

            var byteArray = Encoding.ASCII.GetBytes($"{clientId}:{clientSecret}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
            request.Content = content;

            var response = await httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                if (doc.RootElement.TryGetProperty("access_token", out var tokenElement))
                {
                    return tokenElement.GetString();
                }
            }

            return null;
        }


        public async Task<(T? ModelData, int? StatusCode, string? CompleteResponse, bool? IsSuccessStatusCode)> PostAsync<T>(RequestType _RequestType, HttpContextApiData httpContextData, string source, ApiUrl URL, object param, List<ApiHeaders>? apiHeaders = null, LogKeys? logKeys = null)
        {
            T? Result = default;
            string ResultData = "";
            int StatusCode = 0;
            bool IsSuccessStatusCode = false;
            LogApiRequestResponse logs = new LogApiRequestResponse();
            try
            {
                if (logKeys is null)
                {
                    logKeys = new LogKeys();
                }
                logs.Guid = _guidService.GUID();
                logs.JourneyId = httpContextData.LoginGuid;
                logs.Source = source;
                logs.Url = URL.URL;
                logs.PublicIp = httpContextData.IPAddress;
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(param, source, logs.Guid, "Request"));

                var httpClient = _httpClientFactory.CreateClient("client");
                //Remove this section once token is generated from API
                //if (source.Contains("Screening"))
                //{
                //    string token = "eyJjdHkiOiJKV1QiLCJlbmMiOiJBMjU2R0NNIiwiYWxnIjoiUlNBLU9BRVAtMjU2In0.PqxhJQS5oSPbpsyfAvOtsFWS3_pdzXMn50c8K_opjPuY2ufBlsiKjh9vVFJxj5yLVR54Bmd-bWfSUZPpS1DCSqHKsG4OYso_y4PQGuoXGROfKNwlgR4R6e12BecHMioiRIhetrpaYEHdQtV2vHPk1bwD_8vmXfQmBZXFkFvBGd7W02ay7Zdn33tM333I4ESn-PM7qwl16MmxMNhAvA4NLPBjmZxzwl1z22CuTEcLnmUGgn4_UmeIbmfvQSs2SLkJNlqcKVbhey8Shk0uU98-1DHzDdc6neotlwVwV55ASE-cLrGm3u2eQqbj5clgCvh6W2EA_68ldABf_MdeO7lH7A.HGosx7D2k2xtIagA.iiZlbug1YwO1t0FNMLH2f7V9j1c3v4OuWVy2SOlj0PDh-VDsNXd79XwFoBXCAWbKyw3BC_LQXrX41I0Y4I6UOhrljkaVKWuCbGD27xt_d6AZiCRrRkBSehsR4QBQV328mOYwy7W7cmYe3T2BlgMQGB3O870oSEU_64ibPPSEjTki2bPIzcDA98_UCZnK0nK1d5gap2s6Qt9YWFHa0aaCouOUPb_89leMmG3ZiGVe4E87mR5cusWuutbXu7klNOZ_xlwVFXMzy4ZZ_T-BX0xSMYp0luo93y7LMdipGYEpC1w7PG9bQ8T5GP7byetWj_36unjARdr5p7-U2mgJId8vI2m-GIlvlzNRPLJBKjH3GbCJZMjWPQzUP8GxPgU39b3wbHJPEEbaKZ8FEkr1pBlR0LLf_C0wWTthOh14QW2r2iXqrl1aTb5UeCNDoyHzGtZgfQeZ0I1DbJhYJ8NG-Co7krTXwA5uFXBZXlAZRpMBqfEDrZTlPS0yhHYQyBK22AJKrq2_DlOnq0sq4ZD3IHwqsc8cKNqWZTzxO0DcJiySY8ZbPAiQqnkENRLJOOPSykUe-H0xeEqS8Q_B9dF5r_mPasP1s0keaRkefwIXU3yaf8LpeXDRgK2fETJTazJJR7973ddvTg8Xl5jzzWKYL9GSETs5tPMzjce7zzKeHW4i1oTZP5Hz_Gf_d3or9HXgMCH7S-Zbt0j1pyg4WjZfLNZtSixmxpJX3JRhTQULqdNYAqODSTbEYGeMe6QkZuJuiGFq6gJwCQK0mUBCdm4HbPQ9q_al-bMjaZKM9B7eQWsPmdnceSaIf2HW_A.lUDPOp0g7yi0j3D4_wSt2w";
                //    httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                //}
                if (source.Contains("Screening"))
                {
                    var token = await GetAccessTokenAsync();
                    if (!string.IsNullOrEmpty(token))
                    {
                        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                    }
                    else
                    {
                        logs.Response = "Failed to retrieve access token";
                        logs.StatusCode = "401";
                        await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(logs, source, logs.Guid, "Response"));
                        return (default, 401, "Token generation failed", false);
                    }
                }
                //End remove section
                //if (source.Contains("Nadra"))
                //{
                //    httpClient.DefaultRequestHeaders.Add("SOAPAction", "http://NADRA.Biometric.Verification/IBioVeriSysDigital/GetLastCitizenVerificationResult");
                //}

                StringContent? stringRequest;
                if (_RequestType == RequestType.SOAP)
                {
                    stringRequest = new StringContent(param.ToString() ?? "", Encoding.UTF8, MediaTypeNames.Text.Xml);
                }
                else
                {
                    stringRequest = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, MediaTypeNames.Application.Json);
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                }

                if (apiHeaders?.Count > 0)
                {
                    foreach (var Header in apiHeaders)
                    {
                        httpClient.DefaultRequestHeaders.Add(Header.Key, Header.Value);
                    }
                }
                logs.RequestDateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff");
                logs.Request = JsonConvert.SerializeObject(param);
                using (var response = (_RequestType == RequestType.GET) ? await httpClient.GetAsync(URL.URL).ConfigureAwait(false) : await httpClient.PostAsync(URL.URL, stringRequest))
                {
                    logs.ResponseDateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff");
                    StatusCode = (int)response.StatusCode;
                    ResultData = await response.Content.ReadAsStringAsync();
                    logs.StatusCode = StatusCode.ToString();
                    logs.Response = ResultData;
                    await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogHttpClientAPI(logs));
                    await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(ResultData, "{httpContextData?.SourceURL}", logs.Guid, "Response"));

                    if (response.IsSuccessStatusCode)
                    {
                        IsSuccessStatusCode = true;
                        try
                        {
                            if (_RequestType == RequestType.SOAP)
                            {
                                XmlDocument doc = new XmlDocument();
                                doc.LoadXml(ResultData.ToString());
                                ResultData = JsonConvert.SerializeXmlNode(doc);
                                logs.Response = ResultData;
                            }
                            logs.ResponseCode = ExtractKeyValueFromJson(ResultData, new List<string> { logKeys.ResponseCode });
                            logs.ResponseMessage = ExtractKeyValueFromJson(ResultData, new List<string> { logKeys.ResponseMessage, "error" });
                            Result = JsonConvert.DeserializeObject<T>(ResultData);
                        }
                        catch (Exception ex)
                        {
                            logs.ResponseCode = "500";
                            logs.ResponseMessage = ex.Message;
                            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.Exception("HttpClientFactory/PostAsync \n Reason: Due to Model Binding", $"{source} => {URL.URL}", ex));
                            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogHttpClientAPI(logs));
                            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(ResultData, "{httpContextData?.SourceURL}", logs.Guid, "Response"));

                            Result = default;
                            throw;
                        }
                    }
                }
                //await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogHttpClientAPI(logs));
                return (Result, StatusCode, ResultData, IsSuccessStatusCode);
            }
            catch (Exception ex)
            {
                logs.ResponseDateTime = DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss.fff");
                logs.ResponseCode = "500";
                logs.ResponseMessage = ex.Message;
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.Exception("HttpClientFactory/PostAsync \n Reason: Due to Model Binding", $"{source} => {URL.URL}", ex));
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogHttpClientAPI(logs));
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(ResultData, "{httpContextData?.SourceURL}", logs.Guid, "Response"));

                throw;
            }
        }

        private static string ExtractKeyValueFromJson(string source, List<string> keys)
        {
            if (string.IsNullOrWhiteSpace(source) || keys == null || keys.Count == 0)
            {
                return "N/A";
            }

            try
            {
                using JsonDocument document = JsonDocument.Parse(source);
                return FindKeyValue(document.RootElement, new HashSet<string>(keys));
            }
            catch
            {
                return "N/A";
            }
        }

        private static string FindKeyValue(JsonElement element, HashSet<string> keys)
        {
            if (element.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in element.EnumerateObject())
                {
                    if (keys.Contains(property.Name) && !IsNullOrEmpty(property.Value))
                    {
                        return property.Value.ToString();
                    }

                    string result = FindKeyValue(property.Value, keys);
                    if (result != "N/A")
                    {
                        return result;
                    }
                }
            }
            else if (element.ValueKind == JsonValueKind.Array)
            {
                foreach (var item in element.EnumerateArray())
                {
                    string result = FindKeyValue(item, keys);
                    if (result != "N/A")
                    {
                        return result;
                    }
                }
            }

            return "N/A";
        }

        private static bool IsNullOrEmpty(JsonElement element)
        {
            return element.ValueKind == JsonValueKind.Null ||
                   (element.ValueKind == JsonValueKind.String && string.IsNullOrWhiteSpace(element.GetString()));
        }

    }
}
