namespace Banking.FanFinancing.Infrastructure.DBOs
{
    public class OnboardRequestDbo
    {
        public string TransactionId { get; set; } = string.Empty;
        public string VendorName { get; set; } = string.Empty;
        public string FanModel { get; set; } = string.Empty;
        public int? FanQty { get; set; }
        public int? TotalFanCost { get; set; }
        public int? SecurityDeposit { get; set; }
        public int? ReplacementQty { get; set; }
        public int? ReplacementCost { get; set; }
        public string VendorIban { get; set; } = string.Empty;
        public string Tenure { get; set; } = string.Empty;
        public int? OrderAmount { get; set; }
        public string ProductAgreement { get; set; } = string.Empty;
        public int? SalvageAmount { get; set; }
        public int? NetFinanceingAmount { get; set; }
        public int? ProcessingFee { get; set; }
        public string ProfitRate { get; set; } = string.Empty;
        public string Kibor { get; set; } = string.Empty;
        public string MonthlyRate { get; set; } = string.Empty;
        public string MusawamahPrice { get; set; } = string.Empty;
        public string Installment { get; set; } = string.Empty;
        public string PMT { get; set; } = string.Empty;

    }
    public class OnboardResponseDbo
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseDescription { get; set; } = string.Empty;
        public string NetFinancingAmount { get; set; } = string.Empty;
        public string ProcessingFee { get; set; } = string.Empty;
        public string ProfitRate { get; set; } = string.Empty;
        public string Installment { get; set; } = string.Empty;
        public string Tenure { get; set; } = string.Empty;
    }
    public class OnboardingRuleResponseDbo
    {
        public string Symbol { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
    }
}
