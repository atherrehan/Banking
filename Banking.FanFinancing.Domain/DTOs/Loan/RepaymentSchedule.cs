namespace Banking.FanFinancing.Domain.DTOs.Loan
{

    public class LoanRepaymentScheduleRequestDto
    {
        public string customerCNIC { get; set; } = string.Empty;
        public string mobileNumber { get; set; } = string.Empty;
        public string companyName { get; set; } = string.Empty;
        public string LDRefNo { get; set; } = string.Empty;
        public string orderID { get; set; } = string.Empty;
        public string billConsumerNumber { get; set; } = string.Empty;
    }


    public class LoanRepaymentScheduleResponseDto
    {
        public string responseCode { get; set; } = string.Empty;
        public string responseDescription { get; set; } = string.Empty;
        public string? bankSessionID { get; set; }
        public int[]? serialNumber { get; set; }
        public string[]? billingMonths { get; set; }
        public decimal[]? EMI { get; set; }
        public decimal[]? outstandingAmount { get; set; }
    }


}
