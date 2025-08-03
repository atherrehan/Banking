using Banking.FanFinancing.Shared.Enums;
using Banking.FanFinancing.Shared.Helpers;

namespace Banking.FanFinancing.Infrastructure.DBOs
{
    public class GenerateOtpRequestDbo
    {
        public string otp { get; set; } = AutoGenerate.Generate(GenerateEnum.OTP,6);
        public string customerCNIC { get; set; } = string.Empty;
        public string mobileNumber { get; set; } = string.Empty;
    }
    public class GenerateOtpResponseDbo
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseDescription { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
        public string ExpiresIn { get; set; } = string.Empty;
    }
    public class VerifyOtpRequestDbo
    {
        public string OTP { get; set; } = string.Empty;
        //public string miblSessionID { get; set; } = string.Empty;
        public string customerCNIC { get; set; } = string.Empty;
        public string mobileNumber { get; set; } = string.Empty;
    }
    public class VerifyOtpResponseDbo
    {
        public string ResponseCode { get; set; } = string.Empty;
        public string ResponseDescription { get; set; } = string.Empty;
        public string agreementURL { get; set; } = string.Empty;
    }

}
