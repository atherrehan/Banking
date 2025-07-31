namespace Banking.FanFinancing.Shared.Models
{
    public class JwtSettings
    {
        public int Duration { get; set; } = 180;
        public string Secret { get; set; } = string.Empty;
        public string ValidIssuer { get; set; } = string.Empty;
        public string ValidAudience { get; set; } = string.Empty;
        public string ExpiryMinutes { get; set; } = string.Empty;
        public string PrivateKey { get; set; } = string.Empty;
        public string PublicKey { get; set; } = string.Empty;
    }
}
