namespace Banking.FanFinancing.Infrastructure.DBOs
{
    public class DisbursementRequestDbo
    {
        public string OrderRef { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string AgreementStatus { get; set; } = string.Empty;
        public string VendorAcceptanceStatus { get; set; } = string.Empty;
        public string CustomerAcceptanceStatus { get; set; } = string.Empty;
        public string? ModeOfFinancing { get; set; } = string.Empty;
        public string BillConsumerNumber { get; set; } = string.Empty;
        public string BillReferenceNumber { get; set; } = string.Empty;
        public string? VendorIBAN { get; set; } = string.Empty;
        public string FanQty { get; set; } = string.Empty;
        public string TotalFanCost { get; set; } = string.Empty;
        public string Tenure { get; set; } = string.Empty;
        public string OrderAmount { get; set; } = string.Empty;
        public string CustomerCnic { get; set; } = string.Empty;
        public string LdRef { get; set; } = string.Empty;

    }

    public class DisbursementResponseDbo
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseDescription { get; set; } = string.Empty;
        // public string ldRefNo {  get; set; } = string.Empty;
        public string AssignedLimit { get; set; } = string.Empty;
        public string FinancingAmount { get; set; } = string.Empty;
        public string FirstInstallment { get; set; } = string.Empty;
        public string SubsequentInstallment { get; set; } = string.Empty;
        public string AmountDue { get; set; } = string.Empty;
        public string LdRef { get; set; } = string.Empty;
        public string FinancingAccountNumber { get; set; } = string.Empty;
    }

}
