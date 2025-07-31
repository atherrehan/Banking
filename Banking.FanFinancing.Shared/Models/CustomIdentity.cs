using System.Runtime.InteropServices;
using System.Security.Principal;

namespace Banking.FanFinancing.Shared.Models
{
    public class CustomIdentity : IIdentity
    {
        public string? AuthenticationType => "Custom";
        public bool IsAuthenticated => true;
        public string? Name => string.Empty;
        public int? Id { get; set; }
        public string SessionId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Lat { get; set; } = string.Empty;
        public string Long { get; set; } = string.Empty;
        public string JourneyId { get; set; } = string.Empty;
        public CustomIdentity(TokenClaims tokenClaims)
        {
            Id = tokenClaims.Id;
            SessionId = tokenClaims.SessionId;
            UserName = tokenClaims.UserName;
            Password = tokenClaims.Password;
            Lat = tokenClaims.Lat;
            Long = tokenClaims.Long;
            JourneyId = tokenClaims.JourneyId;
        }
    }
}
