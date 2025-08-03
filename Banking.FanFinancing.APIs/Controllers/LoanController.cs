using Banking.FanFinancing.Domain.DTOs.Loan;
using Banking.FanFinancing.Domain.Services.Interface;
using Banking.FanFinancing.Shared.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Banking.FanFinancing.API.Controllers
{
    [EnableRateLimiting("IPPolicy")]
    [Route("[action]")]
    [ApiController]
    public class LoanController(ILoanService _service) : ControllerBase
    {
        [HttpPost]
        [TypeFilter(typeof(ValidateAuthTokenSession))]
        public async Task<IActionResult> LoanInquiry(LoanInquiryRequestDto _request)
        {
            var response = await _service.LoanInquiry(_request);
            return Ok(response);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateAuthTokenSession))]
        public async Task<IActionResult> LoanRepayment(LoanPaymentRequestDto _request)
        {
            var response = await _service.LoanRePayment(_request);
            return Ok(response);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateAuthTokenSession))]
        public async Task<IActionResult> RepaymentSchedule(LoanRepaymentScheduleRequestDto _request)
        {
            var response = await _service.RepaymentSchedule(_request);
            return Ok(response);
        }
    }
}
