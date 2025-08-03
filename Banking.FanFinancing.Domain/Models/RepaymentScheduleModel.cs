namespace Banking.FanFinancing.Domain.Models
{
    public class RepaymentScheduleModel
    {
        public List<int> serials = new List<int>();
        public List<string> months = new List<string>();
        public List<decimal> emi = new List<decimal>();
        public List<decimal> outstandingAmount = new List<decimal>();
    }
}
