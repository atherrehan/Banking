using Banking.FanFinancing.Domain.DTOs.Customer;

namespace Banking.FanFinancing.Domain.Services.Interface
{
    public interface ICustomerService
    {
        Task<EligibiltiyResponseDto> Eligibiltiy(EligibiltiyRequestDto requestDto);

        Task<OnboardResponseDto> Onboard(OnboardRequestDto requestDto);

        Task<GenerateOtpResponseDto> GenerateOtp(GenerateOtpRequestDto requestDto);

        Task<VerifyOtpResponseDto> VerifyOtp(VerifyOtpRequestDto requestDto);

        Task<DisbursementResponseDto> Disbursement(DisbursementRequestDto requestDto);
    }
}
