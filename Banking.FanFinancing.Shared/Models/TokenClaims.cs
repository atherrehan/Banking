namespace Banking.FanFinancing.Shared.Models
{
    public class TokenClaims
    {
        public int? Id { get; set; }
        public string SessionId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Lat { get; set; } = string.Empty;
        public string Long { get; set; } = string.Empty;
        public string JourneyId { get; set; } = string.Empty;
    }
}
