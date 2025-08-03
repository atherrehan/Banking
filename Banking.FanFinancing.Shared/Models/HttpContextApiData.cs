namespace Banking.FanFinancing.Shared.Models
{
    public class HttpContextApiData
    {
        public string GUID { get; set; } = string.Empty;
        public string SourceURL { get; set; } = string.Empty;
        public string IPAddress { get; set; } = string.Empty;
        public string LoginGuid { get; set; } = string.Empty;
        public string Endpoint { get; set; } = string.Empty;

        public HttpContextApiData GetDefaultContextData()
        {
            HttpContextApiData data = new HttpContextApiData();
            data.GUID = Guid.NewGuid().ToString();
            data.SourceURL = "";
            data.IPAddress = "0.0.0.0";
            data.LoginGuid = Guid.NewGuid().ToString();
            data.Endpoint = string.Empty;
            return data;
        }
    }
}
