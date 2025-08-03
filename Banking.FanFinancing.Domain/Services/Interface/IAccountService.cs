using Banking.FanFinancing.Domain.DTOs.Account;

namespace Banking.FanFinancing.Domain.Services.Interface
{
    public interface IAccountService
    {
        Task<LoginResponseDto> AuthenticationToken();
    }
}
