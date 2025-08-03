namespace Banking.FanFinancing.Infrastructure.Entities.Customer
{
    public class KiborResponseEntity
    {
        public string status { get; set; }=string.Empty;
        public string message { get; set; } = string.Empty;
        public KiborData? data { get; set; }
    }

    public class KiborData
    {
        public string ratE_TYPE { get; set; } = string.Empty;
        public string effective_Date { get; set; } = string.Empty;
        public string rate { get; set; } = string.Empty;
    }

}
