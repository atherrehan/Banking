using Banking.FanFinancing.Shared.Models;
using System.Security.Cryptography;
using System.Text;

namespace Banking.FanFinancing.Shared.Helpers
{
    public class Security
    {
        private static readonly Aes encdec;
        public static readonly byte[] IV;
        public static readonly byte[] Key;
        private static string publicKeyRSA = ApplicationStaticConfigurations.RSAPublicKey;
        private static string privateKeyRSA = ApplicationStaticConfigurations.RSAPrivateKey;
        static Security()
        {
            IV = Encoding.UTF8.GetBytes("!)0!8+9=1^X10z#@");
            Key = Encoding.UTF8.GetBytes("1A#56L10(=w59A_!-~I@o!%_AB8Luh||");
            encdec = Aes.Create();
            encdec.BlockSize = 128;
            encdec.KeySize = 256;
            encdec.IV = IV;
            encdec.Key = Key;
            encdec.Padding = PaddingMode.PKCS7;
            encdec.Mode = CipherMode.CBC;
        }
        public static string EncryptAES(string plainText)
        {
            try
            {
                if (string.IsNullOrEmpty(plainText))
                    return "";

                byte[] bytes = Encoding.UTF8.GetBytes(plainText);
                using (ICryptoTransform crypto = encdec.CreateEncryptor(encdec.Key, encdec.IV))
                {
                    byte[] enc = crypto.TransformFinalBlock(bytes, 0, bytes.Length);
                    return Convert.ToBase64String(enc);
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        public static string DecryptAES(string encText)
        {
            try
            {
                if (!string.IsNullOrEmpty(encText))

                {
                    if (encText.Contains("Server="))
                    {
                        return encText;
                    }
                    byte[] bytes = Convert.FromBase64String(encText);
                    using (ICryptoTransform crypto = encdec.CreateDecryptor(encdec.Key, encdec.IV))
                    {
                        byte[] dec = crypto.TransformFinalBlock(bytes, 0, bytes.Length);
                        return Encoding.UTF8.GetString(dec);
                    }
                }
                return "";
            }

            catch (Exception)
            {
                return "";
            }
        }
    }
}
