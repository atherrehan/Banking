using System.ComponentModel;

namespace Banking.FanFinancing.Shared.Enums
{
    public enum ErrorCodesEnum
    {
        [Description("B5502 - Bad Gateway")]  // Bad Gateway
        B5502 = 502,
        [Description("B4400 - Bad Request")]  // Bad Request
        B4400 = 400,
        [Description("B2200 - Invalid Response")] // Invalid Response
        B2200 = 200,
        [Description("B4404 - Resource Not Found")] // Resource not found
        B4404 = 404
    }
}
