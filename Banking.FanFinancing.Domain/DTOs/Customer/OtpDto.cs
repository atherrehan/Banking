namespace Banking.FanFinancing.Domain.DTOs.Customer
{
    public class GenerateOtpRequestDto
    {
        public string? customerCNIC { get; set; } = string.Empty;
        public string? mobileNumber { get; set; } = string.Empty;
        public string mobileOperator { get; set; } = string.Empty;


    }

    public class GenerateOtpResponseDto
    {
        public string responseCode { get; set; } = string.Empty;
        public string responseDescription { get; set; } = string.Empty;
        public string? otp { get; set; } 
        public string? expiresIn { get; set; } 
    }

    public class VerifyOtpRequestDto
    {
        public string customerCNIC { get; set; } = string.Empty;
        public string mobileNumber { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }

    public class VerifyOtpResponseDto
    {
        public string responseCode { get; set; } = string.Empty;
        public string responseDescription { get; set; } = string.Empty;

    }


}
