using System.ComponentModel;

namespace Banking.FanFinancing.Shared.Enums
{
    public enum ResponseCodesEnum
    {
        [Description("A4401 - Unauthorized")]  // NULL Token
        A4401,
        [Description("A4403 - Required Header(s) are missing")]  // NULL Session
        A4403,
        [Description("A4402 - Unauthorized")] //Token Expired
        A4402,        
        [Description("A4405 - Unauthorized User")] //Token not matched
        A4405,
        [Description("A4006 - Request is tempered")] //Token not matched
        A4006,        
        [Description("A101 - Unable to process your request")] //Token not matched
        A101
    }
}
