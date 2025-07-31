using Banking.FanFinancing.Shared.Enums;
using System.Numerics;
using System.Security.Cryptography;

namespace Banking.FanFinancing.Shared.Helpers
{
    public class AutoGenerate
    {
        public static string Generate(GenerateEnum type = GenerateEnum.NA, int? length = 0, string value = "")
        {
            if (type == GenerateEnum.Guid)
            {
                return Guid.NewGuid().ToString();
            }
            else if (type == GenerateEnum.Date)
            {
                string date = DateTime.Now.Day.ToString("00")
                            + DateTime.Now.Month.ToString("00")
                            + DateTime.Now.Year.ToString("0000"); // 4 digits for the year
                return date;
            }
            else if (type == GenerateEnum.Time)
            {
                return DateTime.Now.ToString("HHmmss");
            }
            //else if (type == GenerateEnum.Time)
            //{
            //    return DateTime.Now.ToShortTimeString();
            //}
            else if (type == GenerateEnum.RRN)
            {
                var random = new Random();
                var rrnLength = 4; // Only generate 4 digits
                var rrn = new string(
                    Enumerable.Range(0, rrnLength)
                              .Select(_ => (char)('0' + random.Next(10)))
                              .ToArray()
                );

                return rrn;
            }
            //else if (type == GenerateEnum.RRN)
            //{
            //    var tempVal = Generate(GenerateEnum.Guid);
            //    return Generate(GenerateEnum.Date) + Regex.Replace(tempVal, "[a-zA-Z]", string.Empty).Substring(0, 4);
            //}
            else if (type == GenerateEnum.Numeric)
            {
                byte[] bytes = new byte[10];
                RandomNumberGenerator.Fill(bytes);
                BigInteger val = new BigInteger(bytes);
                if (val < 0)
                {
                    val = -val;
                }
                string numberString = val.ToString().PadLeft(length ?? 0, '0').Substring(0, length ?? 0);
                return numberString;
            }
            else if (type == GenerateEnum.OTP)
            {
                string response = string.Empty;

                Random _rdm = new Random();
                string chars = "5091720684253697082970917203047516773879406162738425";
                response = new string(Enumerable.Repeat(chars, Convert.ToInt32(length)).Select(s => s[_rdm.Next(s.Length)]).ToArray());
                response = new string(Enumerable.Repeat(chars, Convert.ToInt32(length))
                .Select(s => s[_rdm.Next(s.Length)]).ToArray());
                if (length > 2 && !response.Any(x => char.IsDigit(x)) || !response.Any(x => char.IsLetter(x)))
                {
                    response = new string(Enumerable.Repeat(chars, Convert.ToInt32(length))
                    .Select(s => s[_rdm.Next(s.Length)]).ToArray());
                }
                return response;
            }

            return "";
        }
    }
}
