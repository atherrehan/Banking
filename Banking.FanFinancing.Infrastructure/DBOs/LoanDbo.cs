using Banking.FanFinancing.Shared.Enums;
using Banking.FanFinancing.Shared.Helpers;

namespace Banking.FanFinancing.Infrastructure.DBOs
{
    public class LoanRepaymentRequestDbo
    {
        public string CustomerCnic { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string BillConsumerNumber { get; set; } = string.Empty;
        public string RepaymentAccount { get; set; } = string.Empty;
        public string RepaymentAmount { get; set; } = string.Empty;
    }

    public class LoanRepaymentResponseDbo
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseDescription { get; set; } = string.Empty;
        public string OutstandingAmount { get; set; } = string.Empty;
        public string Tenure { get; set; } = string.Empty;
        public string LdRefNo { get; set; } = string.Empty;
    }

    public class LoanInquiryRequestDbo
    {
        public string CustomerCnic { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string OrderRefNo { get; set; } = string.Empty;
    }

    public class LoanInquiryResponseDbo
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseDescription { get; set; } = string.Empty;
        public string BankSessionID { get; set; } = string.Empty;
        public string OutstandingAmount { get; set; } = string.Empty;
        public string OutstandingTenure { get; set; } = string.Empty;
        public string TotalInstallmentPaid { get; set; } = string.Empty;
        public string LastEMIPaidDate { get; set; } = string.Empty;
        public string Tenure { get; set; } = string.Empty;
        public string CustomerRepaymentAccount { get; set; } = string.Empty;
    }

    public class LoanRepaymentScheduleRequestDbo
    {
        public string CustomerCnic { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string LdRefNo { get; set; } = string.Empty;
        public string OrderID { get; set; } = string.Empty;
        public string BillConsumerNumber { get; set; } = string.Empty;
    }


    public class LoanRepaymentScheduleResponseDbo
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseDescription { get; set; } = string.Empty;
        public string BankSessionID { get; set; } = string.Empty;
        public int[]? SerialNumber { get; set; }
        public string[]? BillingMonths { get; set; }
        public decimal[]? Emi { get; set; }
        public decimal[]? OutstandingAmount { get; set; }
    }
    public class LoanRepaymentScheduledResponseDbo
    {
        public int SrNo { get; set; }
        public string BillingMonth { get; set; } = string.Empty;
        public decimal Emi { get; set; }
        public decimal OutstandingAmount { get; set; }
    }


}
