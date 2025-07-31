namespace Banking.FanFinancing.Shared.Exceptions
{
    public class InvalidTokenException : Exception
    {
        public InvalidTokenException(string message = "Unauthorized") : base(message)
        {

        }
    }
}
