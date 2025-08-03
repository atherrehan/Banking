namespace Banking.FanFinancing.Domain.DTOs.Loan
{
    public class LoanPaymentRequestDto
    {
        public string customerCNIC { get; set; } = string.Empty;
        public string mobileNumber { get; set; } = string.Empty;
        public string billConsumerNumber { get; set; } = string.Empty;
        public string repaymentAccount { get; set; } = string.Empty;
        private object _repaymentAmount = string.Empty;

        public object repaymentAmount
        {
            get => _repaymentAmount;
            set
            {
                if (value != null)
                {
                    var stringValue = value?.ToString()?.Replace(",", "");
                    _repaymentAmount = stringValue??"";
                }
                else
                {
                    _repaymentAmount = string.Empty;
                }
            }
        }
    }
    public class LoanPaymentResponseDto
    {
        public string responseCode { get; set; } = string.Empty;
        public string responseDescription { get; set; } = string.Empty;
        public string? transactionRefNo { get; set; } 
        public string? outstandingAmount { get; set; }
        public string? outstandingTenure { get; set; }
    }
}
