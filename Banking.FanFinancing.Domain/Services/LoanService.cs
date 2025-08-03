using Banking.FanFinancing.Domain.DTOs.Loan;
using Banking.FanFinancing.Domain.Models;
using Banking.FanFinancing.Domain.Services.Interface;
using Banking.FanFinancing.Infrastructure.DBOs;
using Banking.FanFinancing.Shared.DbContext;
using Banking.FanFinancing.Shared.Exceptions;
using Banking.FanFinancing.Shared.Helpers;
using Banking.FanFinancing.Shared.Models;
using Banking.FanFinancing.Shared.Services.Interface;
using Microsoft.AspNetCore.Http;
using System.Text.RegularExpressions;

namespace Banking.FanFinancing.Domain.Services
{
    public class LoanService : ILoanService
    {
        #region Constructor
        private readonly IDbContext _dbContext;
        private readonly HttpContext? _httpContext;
        private readonly CustomIdentity? _identity;
        private readonly IBackgroundTaskQueue _backgroundTaskQueue;
        private readonly ILoggerService _loggerService;
        private readonly IGuidService _guidService;

        public LoanService(IDbContext dbContext, IHttpContextAccessor httpContextAccessor, ILoggerService loggerService, IBackgroundTaskQueue backgroundTaskQueue, IGuidService guidService)
        {
            _dbContext = dbContext;
            _httpContext = httpContextAccessor.HttpContext;
            if (_httpContext?.User?.Identity is CustomIdentity customIdentity && customIdentity.IsAuthenticated)
            {
                _identity = customIdentity;
            }
            else
            {
                _identity = default;
            }

            _loggerService = loggerService;
            _backgroundTaskQueue = backgroundTaskQueue;
            _guidService = guidService;
        }
        #endregion

        #region Services
        public async Task<LoanInquiryResponseDto> LoanInquiry(LoanInquiryRequestDto _requestDto)
        {
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(_requestDto, ControllerPath: "Loan/LoanInquiry", _guidService.GUID(), "Request"));
            LoanInquiryResponseDto response = ValidateLoanInquiry(_requestDto, _httpContext);
            if (!response.responseCode.Equals("00"))
            {
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Loan/LoanInquiry", _guidService.GUID(), "Response"));
                return response;
            }

            var result = await _dbContext.GetSingleAsync<LoanInquiryResponseDbo>
                ("usp_get_payment",
                new LoanInquiryRequestDbo()
                {
                    CustomerCnic = _requestDto.customerCNIC,
                    MobileNumber = _requestDto.mobileNumber,
                    CompanyName = _requestDto.companyName,
                    OrderRefNo = _requestDto.orderRefNo
                }
                );
            if (result is null)
            {
                TraceLogger.Log("LoanService", _httpContext);
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response = new() { responseCode = "99", responseDescription = "Model is null" }, "Loan/LoanInquiry", _guidService.GUID(), "Response"));
                throw new NullModelException();
            }
            response = new LoanInquiryResponseDto()
            {
                responseCode = result.ResponseCode,
                responseDescription = result.ResponseDescription
            };

            if (!result.ResponseCode.Equals("200"))
            {
                TraceLogger.Log("LoanService", _httpContext);
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Loan/LoanInquiry", _guidService.GUID(), "Response"));
                return response;
            }
            else
            {
                response.tenure = result.Tenure;
                response.outstandingTenure = result.OutstandingTenure;
                response.customerRepaymentAccount = result.CustomerRepaymentAccount;
                response.lastEMIPaidDate = result.LastEMIPaidDate;
                response.outstandingAmount = result.OutstandingAmount;
                response.totalInstallmentPaid = result.TotalInstallmentPaid;
                response.bankSessionID = _httpContext?.Request?.Headers["bankSessionId"].FirstOrDefault() ?? "";

            }
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Loan/LoanInquiry", _guidService.GUID(), "Response"));
            return response;
        }

        public async Task<LoanPaymentResponseDto> LoanRePayment(LoanPaymentRequestDto _requestDto)
        {
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(_requestDto, ControllerPath: "Loan/LoanRePayment", _guidService.GUID(), "Request"));
            LoanPaymentResponseDto response = ValidateLoanPayment(_requestDto, _httpContext);
            if (!response.responseCode.Equals("00"))
            {
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Loan/LoanRePayment", _guidService.GUID(), "Response"));
                return response;
            }

            var result = await _dbContext.GetSingleAsync<LoanRepaymentResponseDbo>
                ("usp_new_repayment",
                new LoanRepaymentRequestDbo()
                {
                    CustomerCnic = _requestDto.customerCNIC,
                    MobileNumber = _requestDto.mobileNumber,
                    BillConsumerNumber = _requestDto.billConsumerNumber,
                    RepaymentAccount = _requestDto.repaymentAccount,
                    RepaymentAmount = _requestDto.repaymentAmount.ToString() ?? "",
                }
                );
            if (result == null)
            {
                TraceLogger.Log("LoanService", _httpContext);
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response = new() { responseCode = "99", responseDescription = "Model is null" }, "Loan/LoanRePayment", _guidService.GUID(), "Response"));
                throw new NullModelException();
            }
            else if (!result.ResponseCode.Equals("200"))
            {
                TraceLogger.Log("LoanService", _httpContext);
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
                    responseDescription = result.ResponseDescription,
                    transactionRefNo = result.LdRefNo,
                    outstandingAmount = result.OutstandingAmount,
                    outstandingTenure = result.Tenure
                };
            }
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Loan/LoanRePayment", _guidService.GUID(), "Response"));
            return response;
        }

        public async Task<LoanRepaymentScheduleResponseDto> RepaymentSchedule(LoanRepaymentScheduleRequestDto _requestDto)
        {
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(_requestDto, ControllerPath: "Loan/RepaymentSchedule", _guidService.GUID(), "Request"));
            var sessionId = _httpContext?.Request?.Headers["bankSessionId"].FirstOrDefault() ?? "";
            LoanRepaymentScheduleResponseDto response = ValidateRepaymentSchedule(_requestDto, _httpContext);
            if (!response.responseCode.Equals("00"))
            {
                response.bankSessionID = sessionId;
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Loan/RepaymentSchedule", _guidService.GUID(), "Response"));
                return response;
            }

            var result = await _dbContext.GetMultipleSelectsAsync(
                "usp_get_repayment_schedule",
                new LoanRepaymentScheduleRequestDbo()
                {
                    BillConsumerNumber = _requestDto.billConsumerNumber,
                    CustomerCnic = _requestDto.customerCNIC,
                    LdRefNo = _requestDto.LDRefNo,
                    MobileNumber = _requestDto.mobileNumber,
                    CompanyName = _requestDto.companyName,
                    OrderID = _requestDto.orderID
                },
                x => x.Read<GenericResponse>().FirstOrDefault() ?? new(),
                x => x.Read<LoanRepaymentScheduledResponseDbo>().ToList() ?? new()
                );
            if (result is null || result.Count <= 0)
            {
                TraceLogger.Log("LoanService", _httpContext);
                response = new()
                {
                    responseCode = "99",
                    responseDescription = "Response is null",
                    bankSessionID = sessionId
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Loan/RepaymentSchedule", _guidService.GUID(), "Response"));
                return response;
            }
            var genericResponse = new GenericResponse();
            genericResponse = (GenericResponse)result[0];
            var repayments = new List<LoanRepaymentScheduledResponseDbo>();
            repayments = (List<LoanRepaymentScheduledResponseDbo>)result[1];
            if (genericResponse is null)
            {
                TraceLogger.Log("LoanService", _httpContext);
                response = new()
                {
                    responseCode = "99",
                    responseDescription = "Response is null",
                    bankSessionID = sessionId
                };
                await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Loan/RepaymentSchedule", _guidService.GUID(), "Response"));
                return response;
            }


            else if (!genericResponse.ResponseCode.Equals("200"))
            {
                TraceLogger.Log("LoanService", _httpContext);
                response = new()
                {
                    responseCode = genericResponse.ResponseCode,
                    responseDescription = genericResponse.ResponseDescription,
                    bankSessionID = sessionId
                };
            }
            else
            {
                var repaymentSchedule = RepaymentSchedule(repayments);
                if (repayments is not null)
                {
                    response = new()
                    {
                        responseCode = genericResponse.ResponseCode,
                        responseDescription = genericResponse.ResponseDescription,
                        bankSessionID = _httpContext?.Request?.Headers["bankSessionId"].FirstOrDefault() ?? "",
                        billingMonths = repaymentSchedule.months.ToArray(),
                        EMI = repaymentSchedule.emi.ToArray(),
                        outstandingAmount = repaymentSchedule.outstandingAmount.ToArray(),
                        serialNumber = repaymentSchedule.serials.ToArray()
                    };
                }
                else
                {

                    response = new()
                    {
                        responseCode = genericResponse.ResponseCode,
                        responseDescription = genericResponse.ResponseDescription,
                        bankSessionID = sessionId
                    };
                }
            }
            await _backgroundTaskQueue.QueueBackgroundWorkItemAsync(async ct => await _loggerService.LogRequestResponse(response, "Loan/RepaymentSchedule", _guidService.GUID(), "Response"));
            return response;
        }
        #endregion

        #region Validations
        private LoanPaymentResponseDto ValidateLoanPayment(LoanPaymentRequestDto _requestDto, HttpContext? _httpContext)
        {
            LoanPaymentResponseDto responseDto;
            bool IsNullOrEmpty(string val) => string.IsNullOrWhiteSpace(val);
            bool IsValidIBAN(string? value) =>
              !IsNullOrEmpty(value ?? "")
              && value!.Length == 24
              && Regex.IsMatch(value, @"^PK\d{2}[A-Z]{4}\d{16}$");

            if (!IsNullOrEmpty(_requestDto.customerCNIC ?? "") && !Regex.IsMatch(_requestDto.customerCNIC ?? "", @"^[1-9]\d{12}$"))
            {
                TraceLogger.Log("LoanService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "customerCNIC must be 13 digits, numeric, not start with 0, and not empty" };
            }
            if (!IsNullOrEmpty(_requestDto.mobileNumber ?? "") && !Regex.IsMatch(_requestDto.mobileNumber ?? "", @"^0\d{10}$"))
            {
                TraceLogger.Log("LoanService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "customerCNIC must be 13 digits, numeric, not start with 0, and not empty" };
            }
            if (IsNullOrEmpty(_requestDto.billConsumerNumber ?? ""))
            {
                TraceLogger.Log("LoanService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "billConsumerNumber is required" };
            }
            if (string.IsNullOrEmpty(_requestDto?.repaymentAmount?.ToString())
                || !decimal.TryParse(_requestDto.repaymentAmount.ToString(), out _)
                || !Regex.IsMatch(_requestDto?.repaymentAmount?.ToString()??"", @"^(?!0+(\.0{1,2})?$)\d+(\.\d{1,2})?$"))
            {
                TraceLogger.Log("LoanService", _httpContext);
                return responseDto = new()
                {
                    responseCode = "99",
                    responseDescription = "repaymentAmount must be a numeric value with up to two decimal places and greater than zero"
                };
            }

            if (!string.IsNullOrEmpty(_requestDto?.repaymentAccount) && !IsValidIBAN(_requestDto?.repaymentAccount?.ToString()))
            {
                TraceLogger.Log("LoanService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "repaymentAccount must be 24 characters, alphanumeric, start with 'PK' + 2 digits + 4 letters + 16 digits, and no decimal" };
            }

            return responseDto = new() { responseCode = "00", responseDescription = "Success" };
        }

        private LoanInquiryResponseDto ValidateLoanInquiry(LoanInquiryRequestDto _requestDto, HttpContext? _httpContext)
        {
            LoanInquiryResponseDto responseDto;


            bool IsNullOrEmpty(string val) => string.IsNullOrWhiteSpace(val);

            if (IsNullOrEmpty(_requestDto.customerCNIC ?? "") && !Regex.IsMatch(_requestDto.customerCNIC ?? "", @"^[1-9]\d{12}$"))
            {
                TraceLogger.Log("LoanService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "customerCNIC must be 13 digits, numeric, not start with 0, and not empty" };
            }

            if (IsNullOrEmpty(_requestDto.mobileNumber ?? "") && !Regex.IsMatch(_requestDto.mobileNumber ?? "", @"^0\d{10}$"))
            {
                TraceLogger.Log("LoanService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "mobileNumber must be 13 digits, numeric, not start with 0, and not empty" };
            }
            if (IsNullOrEmpty(_requestDto.companyName ?? ""))
            {
                TraceLogger.Log("LoanService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "companyName is required" };
            }
            if (IsNullOrEmpty(_requestDto?.orderRefNo ?? ""))
            {
                TraceLogger.Log("LoanService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "orderRefNo is required" };
            }
            return responseDto = new() { responseCode = "00", responseDescription = "Success" };
        }

        private LoanRepaymentScheduleResponseDto ValidateRepaymentSchedule(LoanRepaymentScheduleRequestDto _requestDto, HttpContext? _httpContext)
        {
            LoanRepaymentScheduleResponseDto responseDto;

            bool IsNullOrEmpty(string val) => string.IsNullOrWhiteSpace(val);

            if (IsNullOrEmpty(_requestDto.customerCNIC ?? "") && !Regex.IsMatch(_requestDto.customerCNIC ?? "", @"^[1-9]\d{12}$"))
            {
                TraceLogger.Log("LoanService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "customerCNIC must be 13 digits, numeric, not start with 0, and not empty" };
            }
            if (IsNullOrEmpty(_requestDto.mobileNumber ?? "") && !Regex.IsMatch(_requestDto.mobileNumber ?? "", @"^0\d{10}$"))
            {
                TraceLogger.Log("LoanService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "mobileNumber must be 13 digits, numeric, not start with 0, and not empty" };
            }
            if (IsNullOrEmpty(_requestDto.companyName ?? ""))
            {
                TraceLogger.Log("LoanService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "companyName is required" };
            }
            if (IsNullOrEmpty(_requestDto?.LDRefNo ?? ""))
            {
                TraceLogger.Log("LoanService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "LDRefNo is required" };
            }
            if (IsNullOrEmpty(_requestDto?.orderID ?? ""))
            {
                TraceLogger.Log("LoanService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "orderID is required" };
            }
            if (IsNullOrEmpty(_requestDto?.billConsumerNumber ?? ""))
            {
                TraceLogger.Log("LoanService", _httpContext);
                return responseDto = new() { responseCode = "99", responseDescription = "billConsumerNumber is required" };
            }
            return responseDto = new() { responseCode = "00", responseDescription = "Success" };
        }
        #endregion

        #region Loan Repayment Operations

        private RepaymentScheduleModel RepaymentSchedule(List<LoanRepaymentScheduledResponseDbo> repayments)
        {
            var response = new RepaymentScheduleModel();
            if (repayments is not null && repayments.Count > 0)
            {
                var temp = new RepaymentScheduleModel();
                temp.serials = new List<int>();
                List<int> serials = new List<int>();
                List<string> months = new List<string>();
                List<decimal> emi = new List<decimal>();
                List<decimal> outstandingAmount = new List<decimal>();
                foreach (var repayment in repayments)
                {
                    temp.serials.Add(repayment.SrNo);
                    temp.months.Add(repayment.BillingMonth);
                    temp.emi.Add(repayment.Emi);
                    temp.outstandingAmount.Add(repayment.OutstandingAmount);
                }
                response.serials = temp.serials;
                response.months = temp.months;
                response.emi = temp.emi;
                response.outstandingAmount = temp.outstandingAmount;
            }
            return response;
        }

        #endregion
    }
}
