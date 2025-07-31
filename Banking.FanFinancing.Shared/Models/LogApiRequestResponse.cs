namespace Banking.FanFinancing.Shared.Models
{
    public class LogApiRequestResponse
    {
        public string ExternalId { get; set; } = string.Empty;
        public string ProcessingCode { get; set; } = string.Empty;
        public string Request { get; set; } = string.Empty;
        public string Response { get; set; } = string.Empty;
        public string Guid { get; set; } = string.Empty;
        public string JourneyId { get; set; } = string.Empty;
        public string RequestDateTime { get; set; } = string.Empty;
        public string ResponseDateTime { get; set; } = string.Empty;
        public string Source { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string StatusCode { get; set; } = string.Empty;
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public string PublicIp { get; set; } = string.Empty;
    }
}
