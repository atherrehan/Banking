namespace Banking.FanFinancing.Shared.Models
{
    //public class ApiResponse<T> where T : class
    public class ApiResponse
    {
        public string responseCode { get; set; } = "00";
        public string responseMessage { get; set; } = "Success";
        //public string? traceId { get; set; }
        //public T? body { get; set; } = default;
    }
}
