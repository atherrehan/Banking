using Banking.FanFinancing.Shared.Enums;
using Banking.FanFinancing.Shared.Helpers;
using Newtonsoft.Json;

namespace Banking.FanFinancing.Shared.Entities
{
    public class PmdRequestEntity
    {
        public string userName { get; set; } = string.Empty;
        public string passwd { get; set; } = string.Empty;
        public string cnic { get; set; } = string.Empty;
        public string msisdn { get; set; } = string.Empty;
        public string transactionId { get; set; } = AutoGenerate.Generate(GenerateEnum.Numeric, 5);
    }
    public class PmdResponseEntity
    {
        [JsonProperty("@xmlns:ax21")]
        public string xmlnsax21 { get; set; } = string.Empty;

        [JsonProperty("@xmlns:xsi")]
        public string xmlnsxsi { get; set; } = string.Empty;

        [JsonProperty("@xsi:type")]
        public string xsitype { get; set; } = string.Empty;

        [JsonProperty("ax21:message")]
        public string ax21message { get; set; } = string.Empty;

        [JsonProperty("ax21:responseCode")]
        public string ax21responseCode { get; set; } = string.Empty;

        [JsonProperty("ax21:status")]
        public string ax21status { get; set; } = string.Empty;
    }

    public class NsVerifyResponse
    {
        [JsonProperty("@xmlns:ns")]
        public string xmlnsns { get; set; } = string.Empty;

        [JsonProperty("ns:return")]
        public PmdResponseEntity? nsreturn { get; set; }
    }

    public class PmdRootEntity
    {
        [JsonProperty("?xml")]
        public Xml? xml { get; set; }

        [JsonProperty("soapenv:Envelope")]
        public SoapenvEnvelope? soapenvEnvelope { get; set; }
    }

    public class SoapenvBody
    {
        [JsonProperty("ns:verifyResponse")]
        public NsVerifyResponse? nsverifyResponse { get; set; }
    }

    public class SoapenvEnvelope
    {
        [JsonProperty("@xmlns:soapenv")]
        public string xmlnssoapenv { get; set; } = string.Empty;

        [JsonProperty("soapenv:Body")]
        public SoapenvBody? soapenvBody { get; set; }
    }

    public class Xml
    {
        [JsonProperty("@version")]
        public string version { get; set; } = string.Empty;

        [JsonProperty("@encoding")]
        public string encoding { get; set; } = string.Empty;
    }
}
