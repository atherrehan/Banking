namespace Banking.FanFinancing.Domain.DTOs.Account
{
    public class LoginResponseDto
    {
        public string? errorMessage { get; set; }
        public string? authToken { get; set; }
        public string? expiresIn { get; set; } 
    }
}
