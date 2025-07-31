namespace Banking.FanFinancing.Shared.Exceptions
{
    public class UnAuthorizeException : Exception
    {
        public UnAuthorizeException(string code = "03", string message = "Your application cannot be processed at the moment. Please try again later") : base(message)
        {

        }
    }
}
