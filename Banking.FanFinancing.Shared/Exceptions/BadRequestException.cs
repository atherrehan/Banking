using Banking.FanFinancing.Shared.Enums;
using Banking.FanFinancing.Shared.Helpers;

namespace Banking.FanFinancing.Shared.Exceptions
{
    public class BadRequestException : Exception
    {
        public string StatusCode { get; private set; }
        public List<string> Body { get; private set; }
        public string message { get; private set; }
        public override string Message { get { return message; } }


        public BadRequestException(int? status, string message = "Bad Request")
        {
            var returnedEnum = (ErrorCodesEnum)Enum.Parse(typeof(ErrorCodesEnum), status.ToString() ?? "");
            this.message = returnedEnum.GetEnumDescription();
            StatusCode = status.ToString() ?? "";
            Body = new List<string>() { this.message };
        }
    }
}
