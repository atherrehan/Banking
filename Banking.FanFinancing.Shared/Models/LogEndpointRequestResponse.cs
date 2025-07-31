namespace Banking.FanFinancing.Shared.Models
{
    public class LogEndpointRequestResponse
    {
        public string ProcessingCode { get; set; } = string.Empty;
        public string Request { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseDescription { get; set; } = string.Empty;
    }
}
