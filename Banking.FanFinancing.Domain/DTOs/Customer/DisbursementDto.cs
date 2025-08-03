using Newtonsoft.Json;

namespace Banking.FanFinancing.Domain.DTOs.Customer
{
    public class DisbursementRequestDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string? transactionId { get; set; } = string.Empty;
        public string? vendorIBAN { get; set; } = string.Empty;
        public string CustomerCNIC { get; set; } = string.Empty;
        public string billConsumerNumber { get; set; } = string.Empty;
        public string billReferenceNumber { get; set; } = string.Empty;
        public object fanQty { get; set; } = string.Empty;
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
        public object tenure { get; set; } = string.Empty;
        public string? modeOfFinancing { get; set; } = string.Empty;
        public string? deliveryDate { get; set; } = string.Empty;
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
        public string productAgreementStatus { get; set; } = string.Empty;
        public string? vendorAcceptanceStatus { get; set; } = string.Empty;
        public string? customerAcceptanceStatus { get; set; } = string.Empty;

    }

    public class DisbursementResponseDto
    {
        public string responseCode { get; set; } = string.Empty;
        public string responseDescription { get; set; } = string.Empty;
        public string? ldRefNo { get; set; } 
        public string? financingAccountNumber { get; set; }
        public string? bank1LinkCode { get; set; } 
        public string? assignedLimit { get; set; }
        public string? totalAmountOfFanFinancing { get; set; } 
        public string? overdueAmount { get; set; } 
        public string? amountDue { get; set; }
        public string? firstMonthlyInstallment { get; set; } 
        public string? subsequentMonthlyInstallment { get; set; } 
        public string? tenure { get; set; } 

    }
}
