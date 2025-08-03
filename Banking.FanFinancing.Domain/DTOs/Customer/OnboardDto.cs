namespace Banking.FanFinancing.Domain.DTOs.Customer
{
    public class OnboardRequestDto
    {
        public string? transactionID { get; set; } = string.Empty;
        public string? vendorName { get; set; } = string.Empty;
        public string? fanModel { get; set; } = string.Empty;
        public object? fanQty { get; set; }
        private object _totalFanCost = string.Empty;

        public object totalFanCost
        {
            get => _totalFanCost;
            set
            {
                if (value != null)
                {
                    var stringValue = value?.ToString()?.Replace(",", "");
                    _totalFanCost = stringValue??"";
                }
                else
                {
                    _totalFanCost = string.Empty;
                }
            }
        }
        private object _upfrontpayment = string.Empty;

        public object upfrontpayment
        {
            get => _upfrontpayment;
            set
            {
                if (value != null)
                {
                    var stringValue = value?.ToString()?.Replace(",", "");
                    _upfrontpayment = stringValue??"";
                }
                else
                {
                    _upfrontpayment = string.Empty;
                }
            }
        }
        public object? replacementQty { get; set; }
        public string? vendorIBAN { get; set; } = string.Empty;
        public string? tenure { get; set; } = string.Empty;
        private object _orderAmount = string.Empty;

        public object orderAmount
        {
            get => _orderAmount;
            set
            {
                if (value != null)
                {
                    var stringValue = value?.ToString()?.Replace(",", "");
                    _orderAmount = stringValue??"";
                }
                else
                {
                    _orderAmount = string.Empty;
                }
            }
        }
        public string? productAgreement { get; set; } = string.Empty;
    }

    public class OnboardResponseDto
    {
        public string responseCode { get; set; } = string.Empty;
        public string responseDescription { get; set; } = string.Empty;
        public string? bankSessionID { get; set; } 
        public string? netFinancingAmount { get; set; } 
        public string? upFrontMusawamahProfit { get; set; } 
        public string? profitRate { get; set; }
        public string? firstMonthInstallment { get; set; } 
        public string? subsequentMonthInstallment { get; set; } 
        public string? tenure { get; set; } 
    }
}
