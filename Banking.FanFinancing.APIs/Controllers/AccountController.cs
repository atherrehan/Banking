using Banking.FanFinancing.Domain.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Banking.FanFinancing.API.Controllers
{
    [EnableRateLimiting("IPPolicy")]
    [Route("/[action]")]
    [ApiController]
    public class AccountController(IAccountService _service) : ControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> GenerateAuth()
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var response = await _service.AuthenticationToken();
            return Ok(response);
        }
       
    }
}
