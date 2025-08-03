namespace Banking.FanFinancing.Domain.Models
{
    public class EligibilityPreMatrics
    {
        public string Response_Code { get; set; } = string.Empty;
        public string Response_Description { get; set; } = string.Empty;
        public decimal? Max_Bill { get; set; }
        public decimal? Avg_Bill { get; set; }
        public int? Late_Bill_Count { get; set; }
        public decimal? Proxy_Income { get; set; }
        public decimal? Income_Threshold { get; set; }
        public decimal? Eligibile_Income { get; set; } 
        public decimal? Assigned_Limit { get; set; } 
        public string Limit_On { get; set; } = string.Empty;
    }
}
