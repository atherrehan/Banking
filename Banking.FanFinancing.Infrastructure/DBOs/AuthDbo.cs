namespace Banking.FanFinancing.Infrastructure.DBOs
{
    public class GenerateAuthRequestDbo
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;

    }

    public class GenerateAuthResponseDbo
    {
        public int? Id { get; set; }
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
    }

    public class UpdateAuthRequestDbo
    {
        public int? Id { get; set; }
        public string Token { get; set; } = string.Empty;
    }
}

