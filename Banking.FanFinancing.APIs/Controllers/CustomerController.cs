using Banking.FanFinancing.Domain.DTOs.Customer;
using Banking.FanFinancing.Domain.Services.Interface;
using Banking.FanFinancing.Shared.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Banking.FanFinancing.API.Controllers
{
    [EnableRateLimiting("IPPolicy")]
    [Route("/[action]")]
    [ApiController]
    public class CustomerController(ICustomerService _service) : ControllerBase
    {
        [HttpPost]
        [TypeFilter(typeof(ValidateAuthToken))]
        public async Task<IActionResult> CustomerEligibility(EligibiltiyRequestDto _request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _service.Eligibiltiy(_request);
            return Ok(response);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateAuthTokenSession))]
        public async Task<IActionResult> CustomerOnboarding(OnboardRequestDto _request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _service.Onboard(_request);
            return Ok(response);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateAuthTokenSession))]
        [Route("/GenerateOtp")]
        public async Task<IActionResult> Generate(GenerateOtpRequestDto _request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _service.GenerateOtp(_request);
            return Ok(response);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateAuthTokenSession))]
        [Route("/VerifyOtp")]
        public async Task<IActionResult> Verify(VerifyOtpRequestDto _request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _service.VerifyOtp(_request);
            return Ok(response);
        }

        [HttpPost]
        [TypeFilter(typeof(ValidateAuthTokenSession))]
        public async Task<IActionResult> LoanDisbursement(DisbursementRequestDto _request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var response = await _service.Disbursement(_request);
            return Ok(response);
        }
    }
}
