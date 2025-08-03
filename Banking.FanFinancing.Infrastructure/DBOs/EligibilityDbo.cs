namespace Banking.FanFinancing.Infrastructure.DBOs
{
    public class EligibilityRequestDbo
    {
        public string ReferenceNo { get; set; } = string.Empty;
        public string CustomerCNIC { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string MobileNumber { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string BillConsumerNumber { get; set; } = string.Empty;
        public string ConsumerName { get; set; } = string.Empty;
        public string FatherName { get; set; } = string.Empty;
        public string CnicIssuanceDate { get; set; } = string.Empty;
        public int? MonthlyIncome { get; set; }
        public string MeterNumber { get; set; } = string.Empty;
        public string MeterInstallationDate { get; set; } = string.Empty;
        public string CustomerAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string ResidentialStatus { get; set; } = string.Empty;
        public string ModeOfFinancing { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string AverageBill { get; set; } = string.Empty;
        public int? LatePayments { get; set; }
        public string ProxyIncome { get; set; } = string.Empty;
        public string MaxBillAmount { get; set; } = string.Empty;
        public string Threshold { get; set; } = string.Empty;
        public string EligibileIncome { get; set; } = string.Empty;
        public string AllowedLimit { get; set; } = string.Empty;
        public string LimitOn { get; set; } = string.Empty;
        public string SessionId { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;

    }
    public class EligibilityResponseDbo
    {
        public string BankSessionId { get; set; } = string.Empty;
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseDescription { get; set; } = string.Empty;
    }
    public class NadraPmdStatusResponseDbo
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseMessage { get; set; } = string.Empty;
        public bool? NadraVerified { get; set; }
        public bool? PmdVerified { get; set; }
        public string CnicExpiry { get; set; } = string.Empty;
        public string BirthDate { get; set; } = string.Empty;
    }

}
