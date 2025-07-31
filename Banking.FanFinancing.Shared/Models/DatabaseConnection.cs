using Banking.FanFinancing.Shared.Helpers;

namespace Banking.FanFinancing.Shared.Models
{
    public class DatabaseConnection
    {
        public string AlBarakaAppConnectionString { get { return DataBaseConnectionStringDecrypt; } set { DataBaseConnectionStringDecrypt = Security.DecryptAES(value); } }
        private string DataBaseConnectionStringDecrypt = string.Empty;

        public string AlBarakaAuditConnectionString { get { return AuditConnectionStringDecrypt; } set { AuditConnectionStringDecrypt = Security.DecryptAES(value); } }
        private string AuditConnectionStringDecrypt = string.Empty;
    }
}
