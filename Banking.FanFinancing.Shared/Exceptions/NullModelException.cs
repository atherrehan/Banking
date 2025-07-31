namespace Banking.FanFinancing.Shared.Exceptions
{
    public class NullModelException : Exception
    {
        public NullModelException(string message = "Response is null") : base(message)
        {

        }
    }
}
