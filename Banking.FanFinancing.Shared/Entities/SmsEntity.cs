using Banking.FanFinancing.Shared.Enums;
using Banking.FanFinancing.Shared.Helpers;
using System.Xml.Serialization;

namespace Banking.FanFinancing.Shared.Entities
{
    public class SmsRequestEntity
    {
        public string MobileNo { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string TranType { get; set; } = "004";
        public string TransactionNature { get; set; } = "PUSH";
        public string Otp { get; set; } = string.Empty;
        public string Date { get; set; } = AutoGenerate.Generate(GenerateEnum.Date);
        public string Time { get; set; } = AutoGenerate.Generate(GenerateEnum.Time);
        public string Rrn { get; set; } = AutoGenerate.Generate(GenerateEnum.RRN);
    }
    [XmlRoot(ElementName = "return")]
    public class SmsResponseEntity
    {

        [XmlElement(ElementName = "message")]
        public string Message { get; set; } = string.Empty;

        [XmlElement(ElementName = "code")]
        public string Code { get; set; } = string.Empty;

        [XmlElement(ElementName = "status")]
        public string Status { get; set; } = string.Empty;

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; } = string.Empty;

        [XmlAttribute(AttributeName = "ax21")]
        public string Ax21 { get; set; } = string.Empty;

        [XmlAttribute(AttributeName = "xsi")]
        public string Xsi { get; set; } = string.Empty;

        [XmlText]
        public string Text { get; set; } = string.Empty;
    }

    [XmlRoot(ElementName = "verifyResponse")]
    public class SmsVerifyResponse
    {

        [XmlElement(ElementName = "return")]
        public SmsResponseEntity? Return { get; set; }

        [XmlAttribute(AttributeName = "ns")]
        public string Ns { get; set; } = string.Empty;

        [XmlText]
        public string Text { get; set; } = string.Empty;
    }

    [XmlRoot(ElementName = "Body")]
    public class SmsResponseBody
    {

        [XmlElement(ElementName = "verifyResponse")]
        public SmsVerifyResponse? VerifyResponse { get; set; }
    }

    [XmlRoot(ElementName = "Envelope")]
    public class SmsRootEntity
    {

        [XmlElement(ElementName = "Body")]
        public SmsResponseBody? Body { get; set; }

        [XmlAttribute(AttributeName = "soapenv")]
        public string Soapenv { get; set; } = string.Empty;

        [XmlText]
        public string Text { get; set; } = string.Empty;
    }
}
