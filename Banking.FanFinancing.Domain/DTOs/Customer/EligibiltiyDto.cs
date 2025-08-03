namespace Banking.FanFinancing.Domain.DTOs.Customer
{
    public class EligibiltiyRequestDto
    {
        public string? customerCNIC { get; set; } = string.Empty;
        public string? transactionID { get; set; } = string.Empty;
        public string? mobileNumber { get; set; } = string.Empty;
        public string? mobileOperator { get; set; } = string.Empty;
        public string companyName { get; set; } = string.Empty;
        public string? billReferenceNumber { get; set; } = string.Empty;
        public string? billConsumerNumber { get; set; } = string.Empty;
        public string? consumerName { get; set; } = string.Empty;
        public string? fatherName { get; set; } = string.Empty;
        public string? cnicIssuanceDate { get; set; } = string.Empty;
        private object _monthlyIncome = string.Empty;

        public object monthlyIncome
        {
            get => _monthlyIncome;
            set
            {
                if (value != null)
                {
                    var stringValue = value?.ToString()?.Replace(",", "");
                    _monthlyIncome = stringValue??"";
                }
                else
                {
                    _monthlyIncome = string.Empty;
                }
            }
        }
        public string? meterNumber { get; set; } = string.Empty;
        public string? meterInstallationDate { get; set; } = string.Empty;
        public string? customerAddress { get; set; } = string.Empty;
        public string? city { get; set; } = string.Empty;
        public string? residentialStatus { get; set; } = string.Empty;
        public string? modeOfFinancing { get; set; } = string.Empty;
        public string? bankName { get; set; } = string.Empty;
        private object[]? _billAmount;

        public object[]? billAmount
        {
            get => _billAmount;
            set
            {
                if (value == null)
                {
                    _billAmount = null;
                }
                else
                {
                    _billAmount = value
                        .Select(item => item?.ToString()?.Replace(",", ""))
                        .Cast<object>()
                        .ToArray();
                }
            }
        }
        public string[]? billMonth { get; set; }
        public string[]? billingDate { get; set; }
        public string[]? dueDate { get; set; }
        private object[]? _actualPaymentAmount;

        public object[]? actualPaymentAmount
        {
            get => _actualPaymentAmount;
            set
            {
                if (value == null)
                {
                    _actualPaymentAmount = null;
                }
                else
                {
                    _actualPaymentAmount = value
                        .Select(item => item?.ToString()?.Replace(",", ""))
                        .Cast<object>()
                        .ToArray();
                }
            }
        }
        public string[]? actualPaymentDate { get; set; }
        private object[]? _latePaymentAmount;

        public object[]? latePaymentAmount
        {
            get => _latePaymentAmount;
            set
            {
                if (value == null)
                {
                    _latePaymentAmount = null;
                }
                else
                {
                    _latePaymentAmount = value
                        .Select(item => item?.ToString()?.Replace(",", ""))
                        .Cast<object>()
                        .ToArray();
                }
            }
        }
        public object[]? noOfLatePayments { get; set; }
    }


    public class EligibiltiyResponseDto
    {
        public string responseCode { get; set; } = string.Empty;
        public string responseDescription { get; set; } = string.Empty;//Customer is Eligible for financing 
        public string? bankSessionID { get; set; }
        public string? assignedLimit { get; set; } 
        public string? productAgreement { get; set; } 
    }


}
