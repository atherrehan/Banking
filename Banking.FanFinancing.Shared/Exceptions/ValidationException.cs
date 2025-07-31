namespace Banking.FanFinancing.Shared.Exceptions
{
    public class ValidationException : Exception
    {
        public List<string> modelState { get; set; }
        public string StatusCode { get; private set; }
        public ValidationException(List<string> modelState, string status = "A102", string message = "Validation Error") : base(message)
        {
            this.modelState = modelState;
            this.StatusCode = status;
        }
    }
}
