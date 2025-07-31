using Banking.FanFinancing.Shared.Models;

namespace Banking.FanFinancing.Shared.Services.Interface
{
    public interface IAuthTokenService
    {
        string GenerateToken(TokenClaims claims);

        bool ValidateToken(string token, out TokenClaims tokenClaims);

    }
}
