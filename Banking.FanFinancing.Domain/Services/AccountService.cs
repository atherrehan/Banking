using Banking.FanFinancing.Domain.DTOs.Account;
using Banking.FanFinancing.Domain.Services.Interface;
using Banking.FanFinancing.Infrastructure.DBOs;
using Banking.FanFinancing.Shared.DbContext;
using Banking.FanFinancing.Shared.Enums;
using Banking.FanFinancing.Shared.Exceptions;
using Banking.FanFinancing.Shared.Helpers;
using Banking.FanFinancing.Shared.Models;
using Banking.FanFinancing.Shared.Services.Interface;
using Microsoft.AspNetCore.Http;

namespace Banking.FanFinancing.Domain.Services
{
    public class AccountService : IAccountService
    {
        private readonly IAuthTokenService _authTokenService;
        private readonly IDbContext _dbContext;
        private readonly HttpContext? _httpContext;

        public AccountService(IDbContext dbContext, IHttpContextAccessor httpContextAccessor, IAuthTokenService authTokenService)
        {
            _dbContext = dbContext;
            _authTokenService = authTokenService;
            _httpContext = httpContextAccessor.HttpContext;
        }

        public async Task<LoginResponseDto> AuthenticationToken()
        {
            if (!HelperMethods.ValidateHeaders(_httpContext ?? throw new NullModelException("Required header(s) are missing")))
            {
                throw new CustomException("99", "Required headers(s) are missing");
            }

            LoginResponseDto response;

            var userName = HelperMethods.GetHeaderValue(_httpContext, "UserID");
            var password = HelperMethods.GetHeaderValue(_httpContext, "Password");
            var sessionId = AutoGenerate.Generate(GenerateEnum.Guid);
            var requestDbo = new GenerateAuthRequestDbo()
            {
                UserName = userName,
                Password = password,
                SessionId = sessionId
            };
            var result = await _dbContext.GetSingleAsync<GenerateAuthResponseDbo>("usp_generate_auth", requestDbo);
            if (result == null)
            {
                TraceLogger.Log("AuthenticationToken", _httpContext);
                throw new CustomException("400", "Response is null");
            }
            else if (!result.ResponseCode.Equals("200"))
            {
                TraceLogger.Log("AuthenticationToken", _httpContext);
                response = new()
                {
                    errorMessage = "Invalid credentials"
                };
            }
            else
            {
                var journeyId = AutoGenerate.Generate(GenerateEnum.Guid);
                var token = _authTokenService.GenerateToken(new TokenClaims
                {
                    SessionId = sessionId,
                    UserName = userName,
                    Password = password,
                    Id = result.Id,
                    JourneyId = journeyId
                });
                int expires = 180;
                DateTime expire = DateTime.Now.AddMinutes(expires);
                string formatted = string.Format("{0:HH:mm:ss}.{1:D8}", expire, expire.Ticks % TimeSpan.TicksPerSecond * 100); // ticks to nanoseconds

                response = new()
                {
                    errorMessage = null,
                    authToken = token.ToString(),
                    expiresIn = formatted
                };
                var updateAuthRequestDbo = new UpdateAuthRequestDbo()
                {
                    Id = result.Id,
                    Token = token
                };
                var responseDbo = await _dbContext.GetSingleAsync<GenericResponse>("usp_update_generate_auth", updateAuthRequestDbo);
                if (responseDbo is null)
                {
                    throw new NullModelException();
                }
                else if (!responseDbo.ResponseCode.Equals("200"))
                {
                    TraceLogger.Log("AuthenticationToken", _httpContext);
                    response = new()
                    {
                        errorMessage = responseDbo.ResponseCode
                    };
                }
            }

            return response;
        }
    }
}
