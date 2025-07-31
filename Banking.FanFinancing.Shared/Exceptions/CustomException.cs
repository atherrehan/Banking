namespace Banking.FanFinancing.Shared.Exceptions
{
    public class CustomException : Exception
    {
        public string StatusCode { get; private set; }
        public CustomException(string status="400", string message = "Something went wrong, please try again.") : base(message)
        {
            StatusCode = status;
        }
    }
}
