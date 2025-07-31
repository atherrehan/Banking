namespace Banking.FanFinancing.Shared.Models
{
    public class ApiUrl
    {
        public string ProcessingCode { get; init; } = string.Empty;
        public string URL { get; init; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Soap { get; set; } = string.Empty;
        public string ActionName { get; set; } = string.Empty;
        public bool? EncryptRequest { get; set; }
        public bool? EncryptResponse { get; set; }
    }
}
