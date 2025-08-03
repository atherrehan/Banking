using Banking.FanFinancing.Domain.DTOs.Loan;

namespace Banking.FanFinancing.Domain.Services.Interface
{
    public interface ILoanService
    {
        Task<LoanInquiryResponseDto> LoanInquiry(LoanInquiryRequestDto requestDto);

        Task<LoanPaymentResponseDto> LoanRePayment(LoanPaymentRequestDto requestDto);

        Task<LoanRepaymentScheduleResponseDto> RepaymentSchedule(LoanRepaymentScheduleRequestDto requestDto);

    }


}
