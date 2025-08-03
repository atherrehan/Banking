using System.Data;
using System.Globalization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using Banking.FanFinancing.Domain.DTOs.Customer;
using Banking.FanFinancing.Domain.Services.Interface;
using Banking.FanFinancing.Infrastructure.DBOs;
using Banking.FanFinancing.Infrastructure.Entities.Customer;
using Banking.FanFinancing.Infrastructure.Repository.Interface;
using Banking.FanFinancing.Shared.Helpers;
using Banking.FanFinancing.Shared.Models;
using Banking.FanFinancing.Shared.Services.Interface;
using Banking.FanFinancing.Shared.Enums;
using Banking.FanFinancing.Shared.Repositry.Interfaces;
using Banking.FanFinancing.Shared.Entities;
using Banking.FanFinancing.Shared.DbContext;
using Banking.FanFinancing.Domain.Models;
using Newtonsoft.Json;
using Azure;

namespace Banking.FanFinancing.Domain.Services
{
    public class CustomerService : ICustomerService
    {
        #region Constructor
        private readonly IDbContext _dbContext;
        private readonly ILoggerService _loggerService;
        private readonly IGuidService _guidService;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly HttpContext? _httpContext;
        private readonly CustomIdentity? _identity;
        private readonly ICustomerRepository _customerRepository;
        private readonly ISharedRepository _sharedRepository;

        public CustomerService
            (
            IDbContext dbContext,
            IHttpContextAccessor httpContextAccessor,
            ILoggerService loggerService,
            IGuidService guidService,
            IBackgroundTaskQueue backgroundTaskQueue,
            ICustomerRepository customerRepository,
            ISharedRepository sharedRepository
            )
        {
            _dbContext = dbContext;
            _httpContext = httpContextAccessor.HttpContext;
            _loggerService = loggerService;
            _guidService = guidService;
            _backgroundTaskQueue = backgroundTaskQueue;
            _sharedRepository = sharedRepository;
            if (_httpContext?.User?.Identity is CustomIdentity customIdentity && customIdentity.IsAuthenticated)
            {
                _identity = customIdentity;
            }
            else
            {
                _identity = default;
            }
            _customerRepository = customerRepository;

        }
        #endregion

        #region Services

        public async Task<EligibiltiyResponseDto> Eligibiltiy(EligibiltiyRequestDto _requestDto)
        {
            EligibiltiyResponseDto response;
            var sessionId = Guid.NewGuid().ToString(); //_identity?.SessionId ?? "";
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(_requestDto, ControllerPath: "Customer/Eligibility", _guidService.GUID(), "Request"));

            //Fields validation
            response = ValidateEligibility(_requestDto, _httpContext);

            if (!response.responseCode.Equals("00"))
            {
                response.bankSessionID = sessionId ?? "";
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Eligibility", _guidService.GUID(), "Response"));
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogEndpoint(new LogEndpointRequestResponse()
                {
                    ProcessingCode = "Eligibility",
                    ResponseCode = response.responseCode,
                    ResponseDescription = response.responseDescription,
                    Request = JsonConvert.SerializeObject(_requestDto),
                    Response = JsonConvert.SerializeObject(response)
                }));

                return response;
            }

            //Eligibility Criteria
            var preMatrics = EligibilityPreMatrics(_requestDto);

            if (!preMatrics.Response_Code.Equals("00"))
            {
                response = new()
                {
                    responseCode = preMatrics.Response_Code,
                    responseDescription = preMatrics.Response_Description,
                    bankSessionID = sessionId
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Eligibility", _guidService.GUID(), "Response"));
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogEndpoint(new LogEndpointRequestResponse()
                {
                    ProcessingCode = "Eligibility",
                    ResponseCode = response.responseCode,
                    ResponseDescription = response.responseDescription,
                    Request = JsonConvert.SerializeObject(_requestDto),
                    Response = JsonConvert.SerializeObject(response)
                }));

                return response;
            }

            //Nadra and PMD Status
            var nadraPmdStatus = await _dbContext.GetSingleAsync<NadraPmdStatusResponseDbo>
                (
                "usp_validate_pmd_nadra",
                new
                {
                    TransactionId = _requestDto.transactionID,
                    Cnic = _requestDto.customerCNIC,
                    Mobile = _requestDto.mobileNumber
                });

            if (nadraPmdStatus is null)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = "99",
                    responseDescription = "Status not found",
                    bankSessionID = sessionId ?? ""

                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Eligibility", _guidService.GUID(), "Response"));
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogEndpoint(new LogEndpointRequestResponse()
                {
                    ProcessingCode = "Eligibility",
                    ResponseCode = response.responseCode,
                    ResponseDescription = response.responseDescription,
                    Request = JsonConvert.SerializeObject(_requestDto),
                    Response = JsonConvert.SerializeObject(response)
                }));

                return response;
            }
            else if (!nadraPmdStatus.ResponseCode.Equals("00"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = nadraPmdStatus.ResponseCode,
                    responseDescription = nadraPmdStatus.ResponseMessage,
                    bankSessionID = sessionId ?? ""
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Eligibility", _guidService.GUID(), "Response"));
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogEndpoint(new LogEndpointRequestResponse()
                {
                    ProcessingCode = "Eligibility",
                    ResponseCode = response.responseCode,
                    ResponseDescription = response.responseDescription,
                    Request = JsonConvert.SerializeObject(_requestDto),
                    Response = JsonConvert.SerializeObject(response)
                }));

                return response;
            }
            if (nadraPmdStatus.PmdVerified.Equals(false))
            {
                //PMD Api
                bool proceed = true;

                var pmdApi = await _sharedRepository.Pmd
                    (new PmdRequestEntity()
                    {
                        cnic = _requestDto?.customerCNIC ?? "",
                        msisdn = _requestDto?.mobileNumber ?? ""
                    },
                    "CustomerService/Elgibility",
                    _httpContext?.GetHttpContextData() ?? new()
                    );

                if (pmdApi.ModelData is null || !pmdApi.IsSuccessStatusCode.Equals(true))
                {
                    TraceLogger.Log("CustomerService", _httpContext);
                    response = new()
                    {
                        responseCode = "05",
                        responseDescription = "PMD Service Failed",
                        bankSessionID = sessionId
                    };
                    await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Eligibility", _guidService.GUID(), "Response"));
                    await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogEndpoint(new LogEndpointRequestResponse()
                    {
                        ProcessingCode = "Eligibility",
                        ResponseCode = response.responseCode,
                        ResponseDescription = response.responseDescription,
                        Request = JsonConvert.SerializeObject(_requestDto),
                        Response = JsonConvert.SerializeObject(response)
                    }));

                    return response;
                }
                else if (!pmdApi.ModelData.ax21status.Equals("00"))
                {
                    TraceLogger.Log("CustomerService", _httpContext);
                    response = new()
                    {
                        responseCode = pmdApi.ModelData.ax21status.ToString() ?? "05",
                        responseDescription = "Pmd verification failed",//pmdApi.ModelData.ax21message,
                        bankSessionID = sessionId ?? ""
                    };
                    proceed = false;
                }
                else if (!pmdApi.ModelData.ax21responseCode.Equals("01"))
                {
                    TraceLogger.Log("CustomerService", _httpContext);
                    response = new()
                    {
                        responseCode = pmdApi.ModelData.ax21responseCode.ToString() ?? "05",
                        responseDescription = "Customer is Ineligible for financing"
                    };
                    proceed = false;
                }

                //Save records in DB
                await SavePmdDetail(_requestDto ?? new(), pmdApi.ModelData);

                if (!proceed)
                {
                    TraceLogger.Log("CustomerService", _httpContext);
                    response.bankSessionID = sessionId ?? "";
                    await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Eligibility", _guidService.GUID(), "Response"));
                    await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogEndpoint(new LogEndpointRequestResponse()
                    {
                        ProcessingCode = "Eligibility",
                        ResponseCode = response.responseCode,
                        ResponseDescription = response.responseDescription,
                        Request = JsonConvert.SerializeObject(_requestDto),
                        Response = JsonConvert.SerializeObject(response)
                    }));

                    return response;
                }
            }
            if (nadraPmdStatus.NadraVerified.Equals(false))
            {
                bool proceed = true;
                DateTime cnicDate = DateTime.ParseExact(_requestDto?.cnicIssuanceDate ?? "", "dd-MMM-yy", CultureInfo.InvariantCulture);
                string cnicIssueDate = cnicDate.ToString("dd-MM-yyyy");

                var nadraApi = await _sharedRepository.Nadra
                     (
                     new NadraRequestEntity()
                     {
                         cnic = _requestDto?.customerCNIC ?? "",
                         issueDate = cnicIssueDate
                     },
                     "CustomerService/Elibility", _httpContext?.GetHttpContextData() ?? new());

                if (!nadraApi.IsSuccessStatusCode.Equals(true) || nadraApi.ModelData is null || nadraApi.ModelData.data.responseData is null || nadraApi.ModelData.data.responseData.responseStatus is null)
                {
                    TraceLogger.Log("CustomerService", _httpContext);
                    response = new()
                    {
                        responseCode = "01",
                        responseDescription = "Your application cannot be processed at the moment. Please try again later",
                        bankSessionID = sessionId ?? ""
                    };
                    await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Eligibility", _guidService.GUID(), "Response"));
                    await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogEndpoint(new LogEndpointRequestResponse()
                    {
                        ProcessingCode = "Eligibility",
                        ResponseCode = response.responseCode,
                        ResponseDescription = response.responseDescription,
                        Request = JsonConvert.SerializeObject(_requestDto),
                        Response = JsonConvert.SerializeObject(response)
                    }));

                    return response;
                }

                else if (nadraApi.ModelData.data.responseData.personData is null)
                {
                    TraceLogger.Log("CustomerService", _httpContext);
                    response = new()
                    {
                        bankSessionID = sessionId ?? "",
                        responseCode = nadraApi.ModelData.data.responseData.responseStatus.CODE,
                        responseDescription = nadraApi.ModelData.data.responseData.responseStatus.MESSAGE
                    };
                    proceed = false;
                }
                else if (!nadraApi.ModelData.data.responseData.responseStatus.CODE.Equals("100"))
                {
                    TraceLogger.Log("CustomerService", _httpContext);
                    response = new()
                    {
                        bankSessionID = sessionId ?? "",
                        responseCode = nadraApi.ModelData.data.responseData.responseStatus.CODE,
                        responseDescription = nadraApi.ModelData.data.responseData.responseStatus.MESSAGE
                    };
                    proceed = false;
                }

                else if (!nadraApi.ModelData.data.responseData.personData.expiryDate.ToLower().Contains("life"))
                {
                    if (IsCnicExpired(nadraApi.ModelData.data.responseData.personData.expiryDate))
                    {
                        response = new()
                        {
                            responseCode = "02",
                            responseDescription = "You are not allowed to proceed further on expired CNIC.",
                            bankSessionID = sessionId ?? ""
                        };
                        proceed = false;
                    }
                }

                //Save record in DB
                await SaveNadraDetail(_requestDto ?? new(), nadraApi.ModelData?.data ?? new Data());

                if (!proceed)
                {
                    await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Eligibility", _guidService.GUID(), "Response"));
                    await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogEndpoint(new LogEndpointRequestResponse()
                    {
                        ProcessingCode = "Eligibility",
                        ResponseCode = response.responseCode,
                        ResponseDescription = response.responseDescription,
                        Request = JsonConvert.SerializeObject(_requestDto),
                        Response = JsonConvert.SerializeObject(response)
                    }));

                    return response;
                }
            }
            else if (!nadraPmdStatus.CnicExpiry.ToLower().Contains("life"))
            {
                if (IsCnicExpired(nadraPmdStatus.CnicExpiry))
                {
                    response = new()
                    {
                        responseCode = "02",
                        responseDescription = "You are not allowed to proceed further on expired CNIC.",
                        bankSessionID = sessionId ?? ""
                    };
                    await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Eligibility", _guidService.GUID(), "Response"));
                    await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogEndpoint(new LogEndpointRequestResponse()
                    {
                        ProcessingCode = "Eligibility",
                        ResponseCode = response.responseCode,
                        ResponseDescription = response.responseDescription,
                        Request = JsonConvert.SerializeObject(_requestDto),
                        Response = JsonConvert.SerializeObject(response)
                    }));

                    return response;
                }
            }
            //End of PMD and Nadra

            // Screening Api
            var screeningRequest = ScreeningRequest(_requestDto ?? new(), nadraPmdStatus);

            var screeningApi = await _customerRepository.Screening(screeningRequest, "CustomerService/Screening", _httpContext?.GetHttpContextData() ?? new());
            if (screeningApi.ModelData is null || !screeningApi.IsSuccessStatusCode.Equals(true))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = "07",
                    responseDescription = "Screening Services not Working",
                    bankSessionID = sessionId ?? ""
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Eligibility", _guidService.GUID(), "Response"));
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogEndpoint(new LogEndpointRequestResponse()
                {
                    ProcessingCode = "Eligibility",
                    ResponseCode = response.responseCode,
                    ResponseDescription = response.responseDescription,
                    Request = JsonConvert.SerializeObject(_requestDto),
                    Response = JsonConvert.SerializeObject(response)
                }));

                return response;
            }
            else if (screeningApi.ModelData.totalMatches > 25)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = "06",
                    responseDescription = "Customer is Ineligible for financing",
                    bankSessionID = sessionId ?? ""
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Eligibility", _guidService.GUID(), "Response"));
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogEndpoint(new LogEndpointRequestResponse()
                {
                    ProcessingCode = "Eligibility",
                    ResponseCode = response.responseCode,
                    ResponseDescription = response.responseDescription,
                    Request = JsonConvert.SerializeObject(_requestDto),
                    Response = JsonConvert.SerializeObject(response)
                }));

                return response;
            }

            //ECIB / Data Check API – Checks for credit flags (30 DPD, Write - offs)
            var dataCheckRequestEntity = DataCheckRequest(_requestDto ?? new());

            var dataCheckApi = await _customerRepository.DataCheck
                (
                dataCheckRequestEntity,
                "CustomerService/Eligibility",
                _httpContext?.GetHttpContextData() ?? new()
                );
            if (dataCheckApi.ModelData is null || !dataCheckApi.IsSuccessStatusCode.Equals(true))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = "04",
                    responseDescription = "DataCheck Services not Working",
                    bankSessionID = sessionId ?? ""
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Eligibility", _guidService.GUID(), "Response"));
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogEndpoint(new LogEndpointRequestResponse()
                {
                    ProcessingCode = "Eligibility",
                    ResponseCode = response.responseCode,
                    ResponseDescription = response.responseDescription,
                    Request = JsonConvert.SerializeObject(_requestDto),
                    Response = JsonConvert.SerializeObject(response)
                }));

                return response;
            }

            else
            {
                var ccpSummaryList = dataCheckApi.ModelData?.Report?.CCP_SUMMARY_TOTAL;
                bool isUnderReview = false;

                if (ccpSummaryList != null)
                {
                    foreach (var item in ccpSummaryList)
                    {
                        int.TryParse(item.P30, out var p30);
                        int.TryParse(item.P90, out var p90);
                        int.TryParse(item.P120, out var p120);
                        int.TryParse(item.P150, out var p150);
                        int.TryParse(item.P180, out var p180);

                        if (p30 >= 1 || p90 >= 1 || p120 >= 1 || p150 >= 1 || p180 >= 1)
                        {
                            isUnderReview = true;
                            break;
                        }
                    }
                }

                if (isUnderReview)
                {
                    TraceLogger.Log("CustomerService", _httpContext);
                    response = new()
                    {
                        responseCode = "99",
                        responseDescription = "Customer is under review due to past repayment behavior (P30 or higher).",
                        bankSessionID = sessionId ?? ""
                    };
                    await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Eligibility", _guidService.GUID(), "Response"));
                    await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogEndpoint(new LogEndpointRequestResponse()
                    {
                        ProcessingCode = "Eligibility",
                        ResponseCode = response.responseCode,
                        ResponseDescription = response.responseDescription,
                        Request = JsonConvert.SerializeObject(_requestDto),
                        Response = JsonConvert.SerializeObject(response)
                    }));

                    return response;
                }

                #region SummaryComments
                //var summaryArray = dataCheckApi.ModelData.Report.CCP_SUMMARY;
                //var masterArray = dataCheckApi.ModelData.Report.CCP_MASTER;

                //if (summaryArray != null && masterArray != null)
                //{
                //    foreach (var summary in summaryArray)
                //    {
                //        // Find matching CCP_MASTER record by SEQ_NO
                //        var matchedMaster = masterArray.FirstOrDefault(m => m.SEQ_NO == summary.SEQ_NO);

                //        if (matchedMaster != null)
                //        {
                //            // Now validate fields
                //            bool isInvalid =
                //                matchedMaster.ACCT_STATUS != "OPEN" && matchedMaster.ACCT_STATUS != "ACTIVE" &&
                //                matchedMaster.LIMIT == "1" &&
                //                matchedMaster.MEM_NAME == "Private" &&
                //                matchedMaster.LOAN_NO == "1";

                //            if (isInvalid)
                //            {
                //                response = new()
                //                {
                //                    responseCode = "99",
                //                    responseDescription = $"Customer record is under review due to account status: {matchedMaster.ACCT_STATUS}, limit: {matchedMaster.LIMIT}, member name: {matchedMaster.MEM_NAME}."
                //                };
                //                return response;
                //            }
                //        }
                //    }
                //}
                /////******* Below code should not return , uncommenting below code will not save eligbility 
                //// CONTINUE WITH NORMAL FLOW IF NOT UNDER REVIEW
                ////response = new()
                ////{
                ////    responseCode = "00", // Or your success code
                ////    responseDescription = "Customer passed eligibility check."

                //};
                #endregion
            }

            var dt = MonthlyBillDataTable(_requestDto ?? new());

            var result = await SaveEligibility(_requestDto ?? new(), preMatrics, dt, sessionId ?? "");

            if (result == null)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = "99",
                    responseDescription = "Response is null",
                    bankSessionID = result?.BankSessionId ?? ""
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Eligibility", _guidService.GUID(), "Response"));
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogEndpoint(new LogEndpointRequestResponse()
                {
                    ProcessingCode = "Eligibility",
                    ResponseCode = response.responseCode,
                    ResponseDescription = response.responseDescription,
                    Request = JsonConvert.SerializeObject(_requestDto),
                    Response = JsonConvert.SerializeObject(response)
                }));

                return response;
            }
            if (!result.ResponseCode.Equals("00") && !result.ResponseCode.Equals("10"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = result.ResponseCode,
                    responseDescription = result.ResponseDescription,
                    bankSessionID = result?.BankSessionId ?? ""
                };
            }
            else
            {
                response = new EligibiltiyResponseDto
                {
                    responseCode = result.ResponseCode,
                    responseDescription = result.ResponseDescription,
                    bankSessionID = result?.BankSessionId ?? "",
                    assignedLimit = preMatrics.Assigned_Limit?.ToString("0") ?? "",
                    productAgreement = "T&Cs"
                };
            }
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Eligibility", _guidService.GUID(), "Response"));
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogEndpoint(new LogEndpointRequestResponse()
            {
                ProcessingCode = "Eligibility",
                ResponseCode = response.responseCode,
                ResponseDescription = response.responseDescription,
                Request = JsonConvert.SerializeObject(_requestDto),
                Response = JsonConvert.SerializeObject(response)
            }));

            return response;
        }

        public async Task<OnboardResponseDto> Onboard(OnboardRequestDto _requestDto)
        {
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(_requestDto, "Customer/Onboard", _guidService.GUID(), "Request"));
            var sessionId = _httpContext?.Request?.Headers["bankSessionID"].FirstOrDefault()?.ToString() ?? "";//   _identity?.SessionId ?? "";

            OnboardResponseDto response = ValidateOnboarding(_requestDto, _httpContext);
            if (response is null)
            {
                response = new()
                {
                    responseCode = "99",
                    responseDescription = "Response is null",
                    bankSessionID = sessionId ?? ""
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Onboard", _guidService.GUID(), "Response"));
                return response;
            }
            else if (!response.responseCode.Equals("00"))
            {
                response.bankSessionID = sessionId ?? "";
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Onboard", _guidService.GUID(), "Response"));
                return response;
            }

            var rulesDbo = await _dbContext.GetListAsync<OnboardingRuleResponseDbo>("usp_get_onboarding_rules");
            if (rulesDbo is null)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    bankSessionID = sessionId ?? "",
                    responseCode = "99",
                    responseDescription = "Rules not found"
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Onboard", _guidService.GUID(), "Response"));
                return response;
            }

            var kiborApi = await _customerRepository.Kibor("CustomerService/Onboard", _httpContext?.GetHttpContextData() ?? new HttpContextApiData());
            if (kiborApi.ModelData is null || !kiborApi.IsSuccessStatusCode.Equals(true) || kiborApi.ModelData.data is null)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = "08",
                    responseDescription = "Kibor service is unavailable",
                    bankSessionID = sessionId ?? ""
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Onboard", _guidService.GUID(), "Response"));
                return response;
            }
            else if (!kiborApi.ModelData.status.Equals("200"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = kiborApi.ModelData.status,
                    responseDescription = kiborApi.ModelData.message
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Onboard", _guidService.GUID(), "Response"));
                return response;
            }
            var KIBOR = decimal.Parse(kiborApi.ModelData.data.rate.ToString());
            if (string.IsNullOrEmpty(KIBOR.ToString()))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    bankSessionID = sessionId ?? "",
                    responseCode = "99",
                    responseDescription = "Value not found for KIBOR"
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Onboard", _guidService.GUID(), "Response"));
                return response;
            }

            var onboardOperations = OnboardingCalculations(_requestDto, rulesDbo, KIBOR);

            if (!onboardOperations.ResponseCode.Equals("00"))
            {
                response = new()
                {
                    responseCode = onboardOperations.ResponseCode,
                    responseDescription = onboardOperations.ResponseDescription,
                    bankSessionID = sessionId
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Onboard", _guidService.GUID(), "Response"));
                return response;
            }

            var result = await SaveOnboard(_requestDto, onboardOperations);

            if (result == null)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = "99",
                    responseDescription = "Response is null",
                    bankSessionID = sessionId ?? ""
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Onboard", _guidService.GUID(), "Response"));
                return response;
            }
            if (!result.ResponseCode.Equals("200"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    bankSessionID = sessionId ?? "",
                    responseCode = result.ResponseCode,
                    responseDescription = result.ResponseDescription
                };
            }
            else
            {
                response = new()
                {
                    responseCode = result.ResponseCode,
                    responseDescription = result.ResponseDescription,
                    netFinancingAmount = onboardOperations.NFA.ToString() ?? "0",
                    upFrontMusawamahProfit = onboardOperations.MP?.ToString("F2") ?? "0",
                    profitRate = onboardOperations.RATE?.ToString("F2") ?? "0",
                    tenure = _requestDto?.tenure ?? "",
                    firstMonthInstallment = onboardOperations.EMI?.ToString("F2") ?? "0",
                    subsequentMonthInstallment = onboardOperations.PMT?.ToString("F2") ?? "0",
                    bankSessionID = sessionId
                };
            }
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Onboard", _guidService.GUID(), "Response"));

            return response;
        }

        public async Task<GenerateOtpResponseDto> GenerateOtp(GenerateOtpRequestDto _requestDto)
        {
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(_requestDto, "Customer/GenerateOtp", _guidService.GUID(), "Request"));
            GenerateOtpResponseDto response = ValidateGenerateOtp(_requestDto);
            if (response == null)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = "99",
                    responseDescription = "Response is null"
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/GenerateOtp", _guidService.GUID(), "Response"));
                return response;
            }
            else if (!response.responseCode.Equals("00"))
            {
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/GenerateOtp", _guidService.GUID(), "Response"));

                return response;
            }
            GenerateOtpRequestDbo requestDbo = new()
            {
                customerCNIC = _requestDto?.customerCNIC ?? "",
                mobileNumber = _requestDto?.mobileNumber ?? ""
            };

            var result = await _dbContext.GetSingleAsync<GenerateOtpResponseDbo>("usp_new_otp", requestDbo);
            if (result == null)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = "99",
                    responseDescription = "Response is null"
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/GenerateOtp", _guidService.GUID(), "Response"));
                return response;
            }
            else if (!result.ResponseCode.Equals("00"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = result.ResponseCode,
                    responseDescription = result.ResponseDescription
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/GenerateOtp", _guidService.GUID(), "Response"));
                return response;
            }
            var smsRequestEntity = new SmsRequestEntity()
            {
                MobileNo = requestDbo.mobileNumber,
                Otp = requestDbo.otp,
                Message = "Dear customer, your otp is " + requestDbo.otp + ". Please do not share this with anyone. If you have not request this, please call XXXXXXXX."//Move message to config
            };
            var smsApi = await _sharedRepository.Sms(smsRequestEntity, "CustomerService/GenerateOtp", _httpContext?.GetHttpContextData() ?? new());

            if (smsApi.ModelData is null || !smsApi.IsSuccessStatusCode.Equals(true))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = "99",
                    responseDescription = "Response is null"
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/GenerateOtp", _guidService.GUID(), "Response"));
                return response;
            }
            else if (!smsApi.ModelData.Status.Equals("00"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = smsApi.ModelData.Status.ToString() ?? "401",
                    responseDescription = smsApi.ModelData.Message
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/GenerateOtp", _guidService.GUID(), "Response"));
                return response;
            }

            response = new()
            {
                responseCode = result.ResponseCode == "00" ? "200" : result.ResponseCode,
                responseDescription = result.ResponseDescription,
                otp = requestDbo.otp,
                expiresIn = result.ExpiresIn
            };
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/GenerateOtp", _guidService.GUID(), "Response"));
            return response;
        }

        public async Task<VerifyOtpResponseDto> VerifyOtp(VerifyOtpRequestDto _requestDto)
        {
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(_requestDto, "Customer/VerifyOtp", _guidService.GUID(), "Request"));
            VerifyOtpResponseDto response = ValidateVerifyOtp(_requestDto);
            if (response is null)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = "99",
                    responseDescription = "Response is null"
                };

                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/ValidateVerifyOtp", _guidService.GUID(), "Response"));
                return response;
            }
            else if (!response.responseCode.Equals("00"))
            {
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/VerifyOtp", _guidService.GUID(), "Response"));
                return response;
            }
            VerifyOtpRequestDbo requestDbo = new()
            {
                OTP = _requestDto.Otp,
                mobileNumber = _requestDto.mobileNumber,
                customerCNIC = _requestDto.customerCNIC
            };
            var result = await _dbContext.GetSingleAsync<GenerateOtpResponseDbo>("ups_verify_otp", requestDbo);
            if (result == null)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = "99",
                    responseDescription = "Response is null"
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/VerifyOtp", _guidService.GUID(), "Response"));
                return response;
            }
            else if (!result.ResponseCode.Equals("200"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                if (result.ResponseDescription.Contains("Invalid/Expired otp"))
                {
                    result.ResponseCode = "201";
                }
                response = new()
                {
                    responseCode = result.ResponseCode,
                    responseDescription = result.ResponseDescription
                };
            }
            else
            {
                response = new()
                {
                    responseCode = result.ResponseCode,
                    responseDescription = result.ResponseDescription
                };
            }
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/VerifyOtp", _guidService.GUID(), "Response"));
            return response;
        }

        public async Task<DisbursementResponseDto> Disbursement(DisbursementRequestDto _requestDto)
        {
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(_requestDto, "Customer/Disbursement", _guidService.GUID(), "Request"));

            DisbursementResponseDto response = ValidateDisbursement(_requestDto);
            if (response == null)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = "99",
                    responseDescription = "Response is null"
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Disbursement", _guidService.GUID(), "Response"));
                return response;
            }
            else if (!response.responseCode.Equals("00"))
            {
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Disbursement", _guidService.GUID(), "Response"));
                return response;
            }
            var ldRef = "LD" + AutoGenerate.Generate(GenerateEnum.Numeric, 10);
            DisbursementRequestDbo requestDbo = new()
            {
                OrderRef = _requestDto?.OrderId ?? "",
                TransactionId = _requestDto?.transactionId ?? "",
                AgreementStatus = _requestDto?.productAgreementStatus ?? "",
                VendorAcceptanceStatus = _requestDto?.vendorAcceptanceStatus ?? "",
                CustomerAcceptanceStatus = _requestDto?.customerAcceptanceStatus ?? "",//After that, below to match with previously send data
                ModeOfFinancing = _requestDto?.modeOfFinancing ?? "",
                BillConsumerNumber = _requestDto?.billConsumerNumber ?? "",
                CustomerCnic = _requestDto?.CustomerCNIC ?? "",
                BillReferenceNumber = _requestDto?.billReferenceNumber ?? "",
                VendorIBAN = _requestDto?.vendorIBAN ?? "",
                FanQty = _requestDto?.fanQty.ToString() ?? "",
                TotalFanCost = _requestDto?.totalFanCost.ToString() ?? "",
                Tenure = _requestDto?.tenure.ToString() ?? "",
                OrderAmount = _requestDto?.orderAmount.ToString() ?? "",
                LdRef = ldRef
            };
            var result = await _dbContext.GetSingleAsync<DisbursementResponseDbo>("usp_new_disbursement", requestDbo);
            if (result == null)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    responseCode = "99",
                    responseDescription = "Response is null"
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Disbursement", _guidService.GUID(), "Response"));
                return response;
            }
            else if (!result.ResponseCode.Equals("200"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new DisbursementResponseDto
                {
                    responseCode = result.ResponseCode,
                    responseDescription = result.ResponseDescription
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Disbursement", _guidService.GUID(), "Response"));
                return response;
            }
            else
            {
                response = new DisbursementResponseDto
                {
                    responseCode = result.ResponseCode,
                    responseDescription = result.ResponseDescription,
                    assignedLimit = result.AssignedLimit,
                    totalAmountOfFanFinancing = result.FinancingAmount,
                    firstMonthlyInstallment = result.FirstInstallment,
                    subsequentMonthlyInstallment = result.SubsequentInstallment,
                    tenure = _requestDto?.tenure.ToString() ?? "",
                    bank1LinkCode= "0003",
                    //financingAccountNumber = _requestDto?.vendorIBAN ?? "",
                    financingAccountNumber = result.FinancingAccountNumber,
                    amountDue = result.AmountDue,
                    ldRefNo = result.LdRef
                };
            }
            
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Customer/Disbursement", _guidService.GUID(), "Response"));
            return response;
        }

        #endregion

        #region Validation
        private EligibiltiyResponseDto ValidateEligibility(EligibiltiyRequestDto _requestDto, HttpContext? _httpContext)
        {
            EligibiltiyResponseDto responseDto;
            bool IsNullOrEmpty(string val) => string.IsNullOrWhiteSpace(val);
            bool IsAllLetters(string val) => Regex.IsMatch(val ?? "", @"^[A-Za-z\s]+$");
            bool IsNumeric(string val) => Regex.IsMatch(val ?? "", @"^\d+$");
            bool IsAlphanumeric(string val) => Regex.IsMatch(val ?? "", @"^[a-zA-Z0-9]+$");
            bool IsValidDate(string val, string format) => DateTime.TryParseExact(val, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out _);

            if (IsNullOrEmpty(_requestDto.customerCNIC ?? "") || !Regex.IsMatch(_requestDto.customerCNIC ?? "", @"^[1-9]\d{12}$"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "customerCNIC must be 13 digits, numeric, not start with 0, and not empty." };
            }
            if (IsNullOrEmpty(_requestDto.transactionID ?? "") || !IsAlphanumeric(_requestDto?.transactionID ?? ""))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "transactionID must be numeric and not empty." };
            }
            if (IsNullOrEmpty(_requestDto?.mobileNumber ?? "") || !Regex.IsMatch(_requestDto?.mobileNumber ?? "", @"^0\d{10}$"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "mobileNumber must be 11 digits, start with 0, numeric, and not empty." };
            }
            if (IsNullOrEmpty(_requestDto?.mobileOperator ?? ""))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "mobileOperator is required." };
            }

            if (IsNullOrEmpty(_requestDto?.companyName ?? ""))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "companyName is required." };
            }
            if (IsNullOrEmpty(_requestDto?.billReferenceNumber ?? ""))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "billReferenceNumber is required." };
            }
            if (IsNullOrEmpty(_requestDto?.billConsumerNumber ?? ""))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "billConsumerNumber is required." };
            }
            if (IsNullOrEmpty(_requestDto?.consumerName ?? "") || !IsAllLetters(_requestDto?.consumerName ?? ""))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "consumerName must contain only letters and not be empty." };
            }
            if (IsNullOrEmpty(_requestDto?.fatherName ?? "") || !IsAllLetters(_requestDto?.fatherName ?? ""))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "fatherName must contain only letters and not be empty." };
            }
            if (IsNullOrEmpty(_requestDto?.cnicIssuanceDate ?? "") || !IsValidDate(_requestDto?.cnicIssuanceDate ?? "", "dd-MMM-yy"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "cnicIssuanceDate must be in format '24-Mar-25'." };
            }
            if (IsNullOrEmpty(_requestDto?.monthlyIncome.ToString() ?? "") || !IsNumeric(_requestDto?.monthlyIncome.ToString() ?? "") || !Regex.IsMatch(_requestDto?.monthlyIncome.ToString() ?? "", @"^[1-9]\d*$"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "monthlyIncome must be numeric and not empty." };
            }
            if (IsNullOrEmpty(_requestDto?.meterNumber ?? ""))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "meterNumber is required." };
            }

            if (IsNullOrEmpty(_requestDto?.meterInstallationDate ?? "") || !IsValidDate(_requestDto?.meterInstallationDate ?? "", "dd-MMM-yy"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "meterInstallationDate must be in format '24-Mar-25'." };
            }
            DateTime parsedMeterInstallationDate;

            if (DateTime.TryParseExact(_requestDto?.meterInstallationDate, "dd-MMM-yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedMeterInstallationDate))
            {
                bool isAtLeastTwoYearsOld = parsedMeterInstallationDate <= DateTime.Today.AddYears(-2);

                if (!isAtLeastTwoYearsOld)
                {

                    TraceLogger.Log("CustomerService", _httpContext);
                    return responseDto = new() { responseCode = "06", responseDescription = "Customer is Ineligible for financing" };
                }
            }

            if (IsNullOrEmpty(_requestDto?.customerAddress ?? ""))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "customerAddress is required." };
            }
            if (IsNullOrEmpty(_requestDto?.city ?? "") || !IsAllLetters(_requestDto?.city ?? ""))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "city must contain only letters and not be empty." };
            }
            if (IsNullOrEmpty(_requestDto?.residentialStatus ?? "") || !IsAllLetters(_requestDto?.residentialStatus ?? ""))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "residentialStatus must contain only letters and not be empty." };
            }
            if (IsNullOrEmpty(_requestDto?.modeOfFinancing ?? "") || !IsAllLetters(_requestDto?.modeOfFinancing ?? ""))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "modeOfFinancing must contain only letters and not be empty." };
            }
            if (IsNullOrEmpty(_requestDto?.bankName ?? "") )
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "bankName not be empty." };
            }

            var billMonthCount = _requestDto?.billMonth?.Length ?? 0;
            var billAmountCount = _requestDto?.billAmount?.Length ?? 0;
            var billingDateCount = _requestDto?.billingDate?.Length ?? 0;
            var dueDateCount = _requestDto?.dueDate?.Length ?? 0;
            var actualPaymentAmountCount = _requestDto?.actualPaymentAmount?.Length ?? 0;
            var actualPaymentDateCount = _requestDto?.actualPaymentDate?.Length ?? 0;
            var latePaymentAmountCount = _requestDto?.latePaymentAmount?.Length ?? 0;
            var latePaymentcount = _requestDto?.noOfLatePayments?.Length ?? 0;
            if (billMonthCount != 24)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "billMonth missing. Atleast 24 month data is required." };
            }
            if (billAmountCount != 24)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "billAmount missing. Atleast 24 month data is required." };
            }
            if (billingDateCount != 24)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "billingDate missing. Atleast 24 month data is required." };
            }
            if (dueDateCount != 24)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "dueDate missing. Atleast 24 month data is required." };
            }
            if (actualPaymentAmountCount != 24)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "actualPaymentAmount missing. Atleast 24 month data is required." };
            }
            if (actualPaymentDateCount != 24)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "actualPaymentDate missing. Atleast 24 month data is required." };
            }
            if (latePaymentAmountCount != 24)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "latePaymentAmount missing. Atleast 24 month data is required." };
            }
            if (latePaymentcount != 24)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "latePaymentcount missing. Atleast 24 month data is required." };
            }
            return responseDto = new() { responseCode = "00", responseDescription = "Success" };
        }

        private OnboardResponseDto ValidateOnboarding(OnboardRequestDto _requestDto, HttpContext? _httpContext)
        {
            OnboardResponseDto responseDto;
            bool IsNullOrEmpty(string? value) => string.IsNullOrWhiteSpace(value);
            bool IsNumeric(string? value) => decimal.TryParse(value, out _) && !value!.Contains(".");
            bool IsNonZeroNumeric(string? value) => IsNumeric(value) && decimal.Parse(value!) > 0;
            bool IsAlpha(string? value) => Regex.IsMatch(value ?? "", @"^[a-zA-Z\s]+$");

            bool IsValidIBAN(string? value) =>
                !IsNullOrEmpty(value)
                && value!.Length == 24
                && Regex.IsMatch(value, @"^PK\d{2}[A-Z]{4}\d{16}$");

            if (IsNullOrEmpty(_requestDto.transactionID))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "transactionID is required." };
            }
            if (IsNullOrEmpty(_requestDto.vendorName) || !IsAlpha(_requestDto.vendorName))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "vendorName is required and must contain only characters." };
            }
            if (IsNullOrEmpty(_requestDto.fanModel))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "fanModel is required." };
            }
            if (!IsNonZeroNumeric(_requestDto?.fanQty?.ToString()))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "fanQty is required, must be numeric, > 0, and without decimal." };
            }
            if (!IsNonZeroNumeric(_requestDto?.totalFanCost?.ToString()))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "totalFanCost is required, must be numeric, > 0, and without decimal." };
            }
            if (!IsNonZeroNumeric(_requestDto?.upfrontpayment?.ToString()))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "upfrontpayment is required, must be numeric, > 0, and without decimal." };
            }
            if (IsNullOrEmpty(_requestDto?.replacementQty?.ToString()) || !IsNumeric(_requestDto?.replacementQty?.ToString()))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "replacementQty is required and must be numeric without decimal." };
            }
            if (!IsValidIBAN(_requestDto?.vendorIBAN?.ToString()))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "vendorIBAN must be 24 characters, alphanumeric, start with 'PK' + 2 digits + 4 letters + 16 digits, and no decimal." };
            }
            if (!IsNonZeroNumeric(_requestDto?.tenure?.ToString()) ||
                !int.TryParse(_requestDto?.tenure?.ToString(), out var tenureValue) ||
                tenureValue < 6 || tenureValue > 18)
            {

                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "tenure is required, must be numeric, between 6 and 18 and without decimal."
                };
            }
            if (!IsNonZeroNumeric(_requestDto?.orderAmount?.ToString()))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "orderAmount is required, must be numeric, > 0, and without decimal." };
            }
            if (IsNullOrEmpty(_requestDto?.productAgreement?.ToString()) || (_requestDto?.productAgreement?.ToLower() != "yes" && _requestDto?.productAgreement?.ToLower() != "no"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "prodAgreementStatus must be either 'Yes' or 'No'." };
            }
            int FQTY, totalFanCost, replacementQty, orderAmount, TERM;
            if (!int.TryParse(_requestDto?.fanQty?.ToString(), out FQTY))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "Invalid fan quantity"
                };
                return responseDto;
            }

            if (!int.TryParse(_requestDto?.totalFanCost?.ToString(), out totalFanCost))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "Invalid total fan cost"
                };
                return responseDto;
            }


            if (!int.TryParse(_requestDto?.replacementQty?.ToString(), out replacementQty))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "Invalid replacement quantity"
                };
                return responseDto;
            }
            if (!int.TryParse(_requestDto?.orderAmount?.ToString(), out orderAmount))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "Invalid order amount"
                };
                return responseDto;
            }

            if (!int.TryParse(_requestDto?.tenure?.ToString(), out TERM))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "Invalid tenure"
                };
                return responseDto;
            }
            return responseDto = new() { responseCode = "00", responseDescription = "Success" };

        }

        private DisbursementResponseDto ValidateDisbursement(DisbursementRequestDto _requestDto)
        {
            DisbursementResponseDto responseDto;

            bool IsNullOrEmpty(string? value) => string.IsNullOrWhiteSpace(value);
            bool IsNumeric(string? value) => decimal.TryParse(value, out _) && !value!.Contains(".");
            bool IsNonZeroNumeric(string? value) => IsNumeric(value) && decimal.Parse(value!) > 0;
            bool IsAlpha(string? value) => Regex.IsMatch(value ?? "", @"^[a-zA-Z\s]+$");
            bool IsValidIBAN(string? value) =>
                !IsNullOrEmpty(value)
                && value!.Length == 24
                && Regex.IsMatch(value, @"^PK\d{2}[A-Z]{4}\d{16}$");
            bool IsValidDate(string? value) => DateTime.TryParseExact(value, "dd-MMM-yy", CultureInfo.InvariantCulture, DateTimeStyles.None, out _);

            if (IsNullOrEmpty(_requestDto.transactionId))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "transactionId is required."
                };
            }
            if (IsNullOrEmpty(_requestDto.OrderId))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "OrderId is required."
                };
            }
            if (!IsValidIBAN(_requestDto.vendorIBAN))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "vendorIban must be 24 characters, alphanumeric, start with 'PK' + 2 digits + 4 letters + 16 digits, and no decimal."
                };
            }
            if (IsNullOrEmpty(_requestDto.billConsumerNumber))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "billConsumerNumber is required."
                };
            }
            if (IsNullOrEmpty(_requestDto.CustomerCNIC ?? "") || !Regex.IsMatch(_requestDto.CustomerCNIC ?? "", @"^[1-9]\d{12}$"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "customerCNIC must be 13 digits, numeric, not start with 0, and not empty."
                };
            }

            if (IsNullOrEmpty(_requestDto.billReferenceNumber))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "billReferenceNumber is required."
                };
            }
            if (!IsNonZeroNumeric(_requestDto?.fanQty?.ToString()))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "fanQty is required, numeric, > 0, and without decimal."
                };
            }
            if (!IsNonZeroNumeric(_requestDto?.totalFanCost?.ToString()))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "totalFanCost is required, numeric, > 0, and without decimal."
                };
            }
            if (!IsNonZeroNumeric(_requestDto?.tenure?.ToString()) ||
                !int.TryParse(_requestDto?.tenure?.ToString(), out var tenureValue) ||
                tenureValue < 6 || tenureValue > 18)
            {

                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "tenure is required, must be numeric between 6 and 18 and without decimal."
                };
            }

            if (IsNullOrEmpty(_requestDto?.modeOfFinancing?.ToString()) || !IsAlpha(_requestDto?.modeOfFinancing?.ToString()))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "modeOfFinancing is required and must contain only characters."
                };
            }
            if (!IsValidDate(_requestDto?.deliveryDate?.ToString()))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "deliveryDate must be in 'dd-MMM-yy' format."
                };
            }
            if (!IsNonZeroNumeric(_requestDto?.orderAmount?.ToString()))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "orderAmount is required, numeric, > 0, and without decimal."
                };
            }
            if (IsNullOrEmpty(_requestDto?.productAgreementStatus?.ToString()) || _requestDto?.productAgreementStatus?.ToString() != "Yes")
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "agreementStatus must be 'Yes'."
                };
            }
            if (IsNullOrEmpty(_requestDto.vendorAcceptanceStatus) || !(_requestDto.vendorAcceptanceStatus == "Accept" || _requestDto.vendorAcceptanceStatus == "Reject"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "vendorAcceptanceStatus must be either 'Accept' or 'Reject'."
                };
            }
            if (IsNullOrEmpty(_requestDto.customerAcceptanceStatus) || !(_requestDto.customerAcceptanceStatus == "Accept" || _requestDto.customerAcceptanceStatus == "Reject"))
            {
                TraceLogger.Log("OnboDisbursementard", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "customerAcceptanceStatus must be either 'Accept' or 'Reject'."
                };
            }
            return responseDto = new() { responseCode = "00", responseDescription = "Success" };
        }

        private GenerateOtpResponseDto ValidateGenerateOtp(GenerateOtpRequestDto _requestDto)
        {
            GenerateOtpResponseDto responseDto;

            bool IsNullOrEmpty(string? value) => string.IsNullOrWhiteSpace(value);

            if (IsNullOrEmpty(_requestDto.customerCNIC ?? "") || !Regex.IsMatch(_requestDto.customerCNIC ?? "", @"^[1-9]\d{12}$"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "customerCNIC must be 13 digits, numeric, not start with 0, and not empty." };
            }
            if (IsNullOrEmpty(_requestDto.mobileNumber ?? "") || !Regex.IsMatch(_requestDto.mobileNumber ?? "", @"^0\d{10}$"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "mobileNumber must be 11 digits, start with 0, numeric, and not empty." };
            }
            if (IsNullOrEmpty(_requestDto.mobileOperator ?? ""))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "mobileOprator must be provided" };
            }
            return responseDto = new() { responseCode = "00", responseDescription = "Success" };
        }

        private VerifyOtpResponseDto ValidateVerifyOtp(VerifyOtpRequestDto _requestDto)
        {
            VerifyOtpResponseDto responseDto;

            bool IsNullOrEmpty(string? value) => string.IsNullOrWhiteSpace(value);

            if (IsNullOrEmpty(_requestDto.customerCNIC ?? "") || !Regex.IsMatch(_requestDto.customerCNIC ?? "", @"^[1-9]\d{12}$"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "customerCNIC must be 13 digits, numeric, not start with 0, and not empty." };
            }
            if (IsNullOrEmpty(_requestDto.mobileNumber ?? "") || !Regex.IsMatch(_requestDto.mobileNumber ?? "", @"^0\d{10}$"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "mobileNumber must be 11 digits, start with 0, numeric, and not empty." };
            }
            if (IsNullOrEmpty(_requestDto.Otp ?? "") || !Regex.IsMatch(_requestDto.Otp ?? "", @"^[0-9]\d{5}$"))
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "OTP is required." };
            }

            return responseDto = new() { responseCode = "00", responseDescription = "Success" };
        }
        #endregion

        #region Eligibility Operaitons
        private EligibilityPreMatrics EligibilityPreMatrics(EligibiltiyRequestDto _requestDto)
        {
            EligibilityPreMatrics response;

            //Analyze Bill History:
            var billAmounts = _requestDto?.billAmount?
            .Select(x => decimal.Parse(x?.ToString() ?? "", CultureInfo.InvariantCulture))
            .ToList();
            var start12Bills = billAmounts?.Take(12).ToList();
            if (start12Bills is null)
            {
                response = new()
                {
                    Response_Code = "99",
                    Response_Description = "Invalid bills"
                };
                TraceLogger.Log("CustomerService", _httpContext);
                return response;
            }

            decimal maxBill = start12Bills.Max();
            decimal averageBill = start12Bills.Average();

            var noOfLatePaymentAmount = _requestDto?.latePaymentAmount?.Select(x => x?.ToString()).ToList();

            int lateBillAmount = noOfLatePaymentAmount?
                .Where(x => x != null)
                .Select(x => Convert.ToInt32(x))
                .Count(val => val > 0) ?? 0;
            if (lateBillAmount > 2)
            {
                response = new()
                {
                    Response_Code = "99",
                    Response_Description = "Dear Customer you are not eligible for fan replacement program. Please try again later"
                };
                TraceLogger.Log("CustomerService", _httpContext);
                return response;
            }

            var noOfLatePayment = _requestDto?.noOfLatePayments?.Select(x => x?.ToString()).ToList();

            int lateBillCount = noOfLatePayment?
                .Where(x => x != null)
                .Select(x => Convert.ToInt32(x))
                .Count(val => val > 0) ?? 0;
            if (lateBillCount > 2)
            {
                response = new()
                {
                    Response_Code = "06",
                    Response_Description = "Dear Customer you are not eligible for fan replacement program. Please try again later"
                };
                TraceLogger.Log("CustomerService", _httpContext);
                return response;
            }
            //3# Estimate Proxy Income:

            decimal proxyIncome = averageBill / 0.10m; // 10%

            //4# Calculate Eligible Income:

            decimal requiredIncomeThreshold = maxBill * 0.50m;

            if (!decimal.TryParse(_requestDto?.monthlyIncome.ToString(), out decimal monthlyIncome))
            {
                response = new()
                {
                    Response_Code = "99",
                    Response_Description = "Invalid monthly income format"
                };
                TraceLogger.Log("CustomerService", _httpContext);
                return response;
            }
            if (monthlyIncome < requiredIncomeThreshold)
            {
                response = new()
                {
                    Response_Code = "06",
                    Response_Description = "Dear Customer you are not eligible for fan replacement program. Please try again later"

                };
                TraceLogger.Log("CustomerService", _httpContext);
                return response;
            }
            var eligibileIncome = monthlyIncome < proxyIncome ? monthlyIncome : proxyIncome;
            var limitOn = monthlyIncome < proxyIncome ? "Monthly" : "Proxy";

            //5# Assign Financing Limit:
            var assignedLimit = eligibileIncome * 0.50m;
            response = new()
            {
                Response_Code = "00",
                Response_Description = "Success",
                Max_Bill = maxBill,
                Avg_Bill = averageBill,
                Late_Bill_Count = lateBillCount,
                Income_Threshold = requiredIncomeThreshold,
                Proxy_Income = proxyIncome,
                Assigned_Limit = assignedLimit,
                Eligibile_Income = eligibileIncome,
                Limit_On = limitOn
            };
            return response;
        }

        private bool IsCnicExpired(string cnicExpiry)
        {
            DateTime expiryDate = DateTime.ParseExact(cnicExpiry, "yyyy-MM-dd", null);
            DateTime currentDate = DateTime.Today;
            if (expiryDate < currentDate)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                return true;
            }
            return false;
        }

        private async Task SaveNadraDetail(EligibiltiyRequestDto _requestDto, Data _nadraResponseData)
        {
            await _dbContext.ExecuteAsync("usp_new_nadra", new //Move to background
            {
                Cnic = _requestDto?.customerCNIC,
                IssueDate = _requestDto?.mobileNumber,
                Code = _nadraResponseData?.responseData?.responseStatus?.CODE ?? "",
                Message = _nadraResponseData?.responseData?.responseStatus?.MESSAGE,
                Name = _nadraResponseData?.responseData?.personData?.name ?? "",
                MotherName = _nadraResponseData?.responseData?.personData?.motherName ?? "",
                FatherHusbandName = _nadraResponseData?.responseData?.personData?.fatherHusbandName ?? "",
                PresentAddress = _nadraResponseData?.responseData?.personData?.presentAddress ?? "",
                PermanentAddress = _nadraResponseData?.responseData?.personData?.permanentAddress ?? "",
                BirthPlace = _nadraResponseData?.responseData?.personData?.birthPlace ?? "",
                Expiry = _nadraResponseData?.responseData?.personData?.expiryDate ?? "",
                Dob = _nadraResponseData?.responseData?.personData?.dateOfBirth ?? "",
                CardType = ""
            });
        }

        private async Task SavePmdDetail(EligibiltiyRequestDto _requestDto, PmdResponseEntity _pmdModelData)
        {
            await _dbContext.ExecuteAsync("usp_new_pmd", new //Move to background
            {
                Cnic = _requestDto?.customerCNIC,
                Mobile = _requestDto?.mobileNumber,
                Status = _pmdModelData.ax21status,
                ResponseCode = _pmdModelData.ax21responseCode,
                Message = _pmdModelData.ax21message
            });
        }

        private ScreeningRequestEntity ScreeningRequest(EligibiltiyRequestDto _requestDto, NadraPmdStatusResponseDbo _nadraDetails)
        {
            var screeningRequest = new ScreeningRequestEntity
            {
                profileId = "8ee3539b-e9c3-48b4-8964-57fae652be83",
                accountId = 1,
                datasetId = 27,
                correlationId = AutoGenerate.Generate(GenerateEnum.Numeric, 12),
                inputRecord = new List<ScreeningRequestInputRecord>
                {
                    new() { fieldName = "Customer ID", fieldValue = AutoGenerate.Generate(GenerateEnum.Numeric, 8) },
                    new() { fieldName = "Customer Name", fieldValue = _requestDto?.consumerName??"" },
                    new() { fieldName = "Father Name", fieldValue = _requestDto.fatherName },
                    new() { fieldName = "CIF", fieldValue = "" },
                    new() { fieldName = "CNIC/NTN", fieldValue = _requestDto?.customerCNIC },
                    new() { fieldName = "Country of Residence", fieldValue = "Pakistan" },
                    new() { fieldName = "DOB", fieldValue =  _nadraDetails.BirthDate },
                    new() { fieldName = "Passport", fieldValue = "" },
                    new() { fieldName = "Nationality", fieldValue = "Pakistani" },
                    new() { fieldName = "Address", fieldValue = "" },
                    new() { fieldName = "Branch Code", fieldValue = "" },
                    new() { fieldName = "Branch Name", fieldValue = "" },
                    new() { fieldName = "Branch User ID", fieldValue = "" }
                },
                config = new ScreeningRequestConfig
                {
                    storeInput = "Y",
                    responseType = "SIMPLE",
                    entityDetails = "N",
                    showMatchedData = "N"
                }
            };

            return screeningRequest;
        }

        private DataCheckRequestEntity DataCheckRequest(EligibiltiyRequestDto _requestDto)
        {
            var request = new DataCheckRequestEntity()
            {
                phoneNo = _requestDto?.mobileNumber ?? "",
                nicNoOrPassportNo = _requestDto?.customerCNIC ?? "",
                //amount = //confirm which amount to be set, allowed or which
                //firstName= ,
                //middleName=,
                //date lastName=,
                // dateOfBirth = birthDate.Replace("-", "/"),// Set Proper format with datetime. Also, confirm if this is from Nadra record
                //gender=,
                address = _requestDto?.customerAddress ?? "",//Confirm this address from Nadra of from API request
                cityOrDistrict = _requestDto?.city ?? ""//Confirm this city from Nadra (Birth Place) or from API request
            };
            return request;
        }

        private DataTable MonthlyBillDataTable(EligibiltiyRequestDto _requestDto)
        {
            var dt = new DataTable();
            dt.Columns.Add("BillMonth", typeof(string));
            dt.Columns.Add("BillingDate", typeof(string));
            dt.Columns.Add("DueDate", typeof(string));
            dt.Columns.Add("BillAmount", typeof(int));
            dt.Columns.Add("ActualPaymentDate", typeof(string));
            dt.Columns.Add("ActualPaymentAmount", typeof(int));
            dt.Columns.Add("LatePaymentAmount", typeof(int));
            dt.Columns.Add("LatePaymentCount", typeof(int));

            for (int i = 0; i < 24; i++)
            {

                var row = dt.NewRow();
                row["BillMonth"] = _requestDto.billMonth?[i] ?? string.Empty;
                row["BillingDate"] = _requestDto.billingDate?[i] ?? string.Empty;
                row["DueDate"] = _requestDto.dueDate?[i] ?? string.Empty;
                var billAmount = _requestDto.billAmount?[i] ?? "0";

                if (billAmount is null || billAmount.ToString() == "")
                {
                    billAmount = "0";
                }
                row["BillAmount"] = int.Parse(billAmount?.ToString() ?? "0");
                row["ActualPaymentDate"] = _requestDto.actualPaymentDate?[i] ?? string.Empty;
                var actualPaymentAmount = _requestDto.actualPaymentAmount?[i] ?? "0";
                if (actualPaymentAmount is null || actualPaymentAmount.ToString() == "")
                {
                    actualPaymentAmount = "0";
                }
                row["ActualPaymentAmount"] = int.Parse(actualPaymentAmount?.ToString() ?? "0");
                var latePaymentAmount = _requestDto.latePaymentAmount?[i] ?? "0";
                if (latePaymentAmount is null || latePaymentAmount.ToString() == "")
                {
                    latePaymentAmount = "0";
                }
                row["LatePaymentAmount"] = int.Parse(latePaymentAmount?.ToString() ?? "0");
                var latePaymentCount = _requestDto.noOfLatePayments?[i] ?? "0";
                if (latePaymentCount is null || latePaymentCount.ToString() == "")
                {
                    latePaymentCount = "0";
                }
                row["LatePaymentCount"] = int.Parse(latePaymentCount?.ToString() ?? "0");
                dt.Rows.Add(row);
            }
            return dt;
        }

        private async Task<EligibilityResponseDbo> SaveEligibility(EligibiltiyRequestDto _requestDto, EligibilityPreMatrics _preMatrics, DataTable _monthlyBills, string _sessionId)
        {

            var ReferenceNo = new SnowflakeIdGenerator(workerId: 1, datacenterId: 1);
            var authorizationToken = _httpContext.Request.Headers.TryGetValue("AuthToken", out var AuthHeader) ? AuthHeader.ToString() ?? "" : "";

            var result = await _dbContext.GetSingleAsync<EligibilityResponseDbo>("usp_new_eligibiltiy", new
            {
                ConsumerName = _requestDto?.consumerName ?? "",
                MobileNumber = _requestDto?.mobileNumber ?? "",
                CustomerCNIC = _requestDto?.customerCNIC ?? "",
                CnicIssuanceDate = _requestDto?.cnicIssuanceDate ?? "",
                BankName = _requestDto?.bankName ?? "",
                FatherName = _requestDto?.fatherName ?? "",
                MonthlyIncome = int.Parse(_requestDto?.monthlyIncome.ToString() ?? "0"),
                CustomerAddress = _requestDto?.customerAddress ?? "",
                City = _requestDto?.city ?? "",
                ResidentialStatus = _requestDto?.residentialStatus ?? "",
                ModeOfFinancing = _requestDto?.modeOfFinancing ?? "",
                MobileOperator = _requestDto?.mobileOperator ?? "",
                CompanyName = _requestDto?.companyName ?? "",
                BillConsumerNumber = _requestDto?.billConsumerNumber ?? "",
                BillReferenceNumber = _requestDto?.billReferenceNumber ?? "",
                MeterNumber = _requestDto?.meterNumber ?? "",
                MeterInstallationDate = _requestDto?.meterInstallationDate ?? "",
                TransactionId = _requestDto?.transactionID ?? "",
                ReferenceNo = ReferenceNo.NextId().ToString(),
                MaxBillAmount = _preMatrics.Max_Bill?.ToString("0"),
                AverageBill = _preMatrics.Avg_Bill?.ToString("F2"),
                LatePayments = _preMatrics.Late_Bill_Count.ToString(),
                ProxyIncome = _preMatrics.Proxy_Income?.ToString("F2"),
                Threshold = _preMatrics.Income_Threshold?.ToString("0"),
                EligibileIncome = _preMatrics.Eligibile_Income?.ToString("0"),
                AllowedLimit = _preMatrics.Assigned_Limit?.ToString("F2"),
                LimitOn = _preMatrics.Limit_On,
                SessionId = _sessionId,
                Token = authorizationToken,
                BillingDetail = _monthlyBills
            });
            return result ?? new();

        }
        #endregion

        #region Onboarding Operations
        private OnboardingModel OnboardingCalculations(OnboardRequestDto _requestDto, List<OnboardingRuleResponseDbo> _rulesDbo, decimal _kibor)
        {
            var response = new OnboardingModel()
            {
                KIBOR = _kibor
            };
            //1# Net Cost
            response.NC = int.Parse(_requestDto.totalFanCost?.ToString() ?? "0");

            //2# Salvage Value of Replaced Fans (SV)
            response.SV = int.Parse(_requestDto.upfrontpayment?.ToString() ?? "0");

            //3# Net Financing Amount (NFA)
            response.NFA = response.NC - response.SV;
            response.MINFIN = int.Parse(_rulesDbo.FirstOrDefault(x => x.Symbol == "MIN_FIN")?.Value ?? "0");
            response.MAXFIN = int.Parse(_rulesDbo.FirstOrDefault(x => x.Symbol == "MAX_FIN")?.Value ?? "0");
            if (response.MINFIN == 0 || response.MAXFIN == 0)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    ResponseCode = "99",
                    ResponseDescription = "Value not found for Min and Max amount"
                };
                return response;
            }
            if (response.NFA < response.MINFIN || response.NFA > response.MAXFIN)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    ResponseCode = "06",
                    ResponseDescription = "Dear Customer you are not eligible for fan replacement program. Please try again later"
                };
                return response;
            }

            //4# Processing Fee
            response.FEE = int.Parse(_rulesDbo.FirstOrDefault(x => x.Symbol == "PROC_FEE")?.Value ?? "0");
            response.FIRSTINSTFEE = int.Parse(_rulesDbo.FirstOrDefault(x => x.Symbol == "FIRST_INSTALLATION_FEE")?.Value ?? "0");
            response.INSTALLFEE = int.Parse(_rulesDbo.FirstOrDefault(x => x.Symbol == "INSTALLATION_FEE")?.Value ?? "0");
            if (response.FEE == 0)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    ResponseCode = "99",
                    ResponseDescription = "Value not found for PROC_FEE"
                };
                return response;
            }
            /*if (response.INSTALLFEE == 0)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    ResponseCode = "99",
                    ResponseDescription = "Value not found for INSTALLATION_FEE"
                };
                return response;
            }
            if (response.FIRSTINSTFEE == 0)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    ResponseCode = "99",
                    ResponseDescription = "Value not found for FIRST_INSTALLATION_FEE"
                };
                return response;
            }*/
            response.PROCFEE = response.FEE;
            //response.PROCFEE = (int.Parse(_requestDto.fanQty?.ToString() ?? "0") * response.INSTALLFEE) + response.FIRSTINSTFEE + response.FEE;
            response.MARGIN = int.Parse(_rulesDbo.FirstOrDefault(x => x.Symbol == "MARGIN")?.Value ?? "0");
            if (response.MARGIN == 0)
            {
                TraceLogger.Log("CustomerService", _httpContext);
                response = new()
                {
                    ResponseCode = "99",
                    ResponseDescription = "Value not found for MARGIN"
                };
                return response;
            }
            response.RATE = (_kibor + response.MARGIN) / 100M;

            //8# Musawamah Price
            response.PMT = (response.NFA * (response.RATE / 12)) / (1 - (decimal)Math.Pow(1 + ((double)response.RATE / 12), -int.Parse(_requestDto.tenure?.ToString() ?? "0")));

            //9# MusawamahProfit
            response.MP = response.NFA * response.RATE;

            //10# EMI
            //response.EMI = response.PMT + (response.FEE * int.Parse(_requestDto.fanQty?.ToString() ?? "0"));
            response.EMI = response.PMT + response.PROCFEE;
            response.ResponseCode = "00";
            response.ResponseDescription = "Success";
            response.RATE = response.RATE * 100M;
            return response;
        }

        private async Task<OnboardResponseDbo> SaveOnboard(OnboardRequestDto _requestDto, OnboardingModel _onboardOperations)
        {
            OnboardRequestDbo requestDbo = new()
            {
                VendorName = _requestDto?.vendorName ?? "",
                VendorIban = _requestDto?.vendorIBAN ?? "",
                FanModel = _requestDto?.fanModel ?? "",
                FanQty = int.Parse(_requestDto?.fanQty?.ToString() ?? "0"),
                ReplacementQty = int.Parse(_requestDto?.replacementQty?.ToString() ?? "0"),
                OrderAmount = int.Parse(_requestDto?.orderAmount?.ToString() ?? "0"),
                TotalFanCost = int.Parse(_requestDto?.totalFanCost?.ToString() ?? "0"),
                ReplacementCost = _onboardOperations.SV,
                SecurityDeposit = int.Parse(_requestDto?.upfrontpayment?.ToString() ?? "0"),
                Tenure = _requestDto?.tenure ?? "0",
                ProductAgreement = _requestDto?.productAgreement ?? "",
                SalvageAmount = _onboardOperations.SV,
                NetFinanceingAmount = _onboardOperations.NFA,
                ProcessingFee = _onboardOperations.FEE,
                ProfitRate = _onboardOperations.RATE?.ToString("F2") ?? "0",
                Kibor = _onboardOperations.KIBOR?.ToString("F2") ?? "0",
                MonthlyRate = (_onboardOperations.RATE / 12)?.ToString("F2") ?? "0",
                MusawamahPrice = _onboardOperations.MP?.ToString("F2") ?? "0",
                Installment = _onboardOperations.EMI?.ToString("F2") ?? "0",
                PMT = _onboardOperations.PMT?.ToString("F2") ?? "0",
                TransactionId = _requestDto?.transactionID ?? ""
            };
            var result = await _dbContext.GetSingleAsync<OnboardResponseDbo>("usp_new_onboard", requestDbo);
            return result;
        }
        #endregion
    }
}
