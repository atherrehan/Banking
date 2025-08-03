namespace Banking.FanFinancing.Domain.Models
{
    public class OnboardingModel
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseDescription { get; set; } = string.Empty;
        public int? NC { get; set; }
        public int? SV { get; set; }
        public int? NFA { get; set; }
        public int? MINFIN { get; set; }
        public int? MAXFIN { get; set; }
        public int? FEE { get; set; }
        public int? PROCFEE { get; set; }
        public decimal? KIBOR { get; set; }
        public int? MARGIN { get; set; }
        public decimal? RATE { get; set; }
        public decimal? PMT { get; set; }
        public decimal? MP { get; set; }
        public decimal? EMI { get; set; }
        public int? FIRSTINSTFEE { get; set; }
        public int? INSTALLFEE { get; set; }
    }
}
