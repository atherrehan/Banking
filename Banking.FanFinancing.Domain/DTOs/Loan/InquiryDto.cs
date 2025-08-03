namespace Banking.FanFinancing.Domain.DTOs.Loan
{
    public class LoanInquiryRequestDto
    {
        public string customerCNIC { get; set; } = string.Empty;
        public string mobileNumber { get; set; } = string.Empty;
        public string companyName { get; set; } = string.Empty;
        public string orderRefNo { get; set; } = string.Empty;
    }
    public class LoanInquiryResponseDto
    {
        public string responseCode { get; set; } = string.Empty;
        public string responseDescription { get; set; } = string.Empty;
        public string? bankSessionID { get; set; }
        public string? outstandingAmount { get; set; }
        public string? outstandingTenure { get; set; }
        public string? totalInstallmentPaid { get; set; }
        public string? lastEMIPaidDate { get; set; }
        public string? tenure { get; set; }
        public string? customerRepaymentAccount { get; set; }
    }


}
