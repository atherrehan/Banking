namespace Banking.FanFinancing.Shared.Entities
{
    //Nadra Rest Start
    public class NadraRequestEntity
    {
        public string cnic { get; set; } = string.Empty;
        public string issueDate { get; set; } = string.Empty;
    }

    public class NadraResponseEntity
    {
        public Status? status { get; set; }
        public Data? data { get; set; }
    }

    public class Status
    {
        public int code { get; set; }
        public string message { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
    }

    public class Data
    {
        public Responsedata? responseData { get; set; }
    }

    public class Responsedata
    {
        public Responsestatus? responseStatus { get; set; }
        public string citizenNumber { get; set; } = string.Empty;
        public Persondata? personData { get; set; }
    }

    public class Responsestatus
    {
        public string CODE { get; set; } = string.Empty;
        public string MESSAGE { get; set; } = string.Empty;
    }

    public class Persondata
    {
        public string name { get; set; } = string.Empty;
        public string fatherHusbandName { get; set; } = string.Empty;
        public string presentAddress { get; set; } = string.Empty;
        public string permanentAddress { get; set; } = string.Empty;
        public string dateOfBirth { get; set; } = string.Empty;
        public string birthPlace { get; set; } = string.Empty;
        public string photograph { get; set; } = string.Empty;
        public string expiryDate { get; set; } = string.Empty;
        public string motherName { get; set; } = string.Empty;
    }
    //Nadra Rest End

    //public class NadraRequestEntity
    //{
    //    public string FranchiseId { get; set; } = "1121";
    //    public string SessionId { get; set; } = string.Empty;// = AutoGenerate.Generate(GenerateEnum.Numeric, 19);
    //    public string TransactionId { get; set; } = AutoGenerate.Generate(GenerateEnum.Numeric, 19);
    //    public string CitizenNumber { get; set; } = string.Empty;
    //    public string IssueDate { get; set; } = string.Empty;
    //    public string BirthYear { get; set; } = string.Empty;
    //    public string AreaName { get; set; } = string.Empty;
    //}



    //[XmlRoot("VERIFICATION")]
    //public class NadraResponseEntity
    //{
    //    [XmlElement("RESPONSE_DATA")]
    //    public ResponseData? ResponseData { get; set; }
    //}

    //public class ResponseData
    //{
    //    [XmlElement("RESPONSE_STATUS")]
    //    public ResponseStatus? ResponseStatus { get; set; }

    //    [XmlElement("SESSION_ID")]
    //    public string SessionId { get; set; } = string.Empty;

    //    [XmlElement("CITIZEN_NUMBER")]
    //    public string CitizenNumber { get; set; } = string.Empty;

    //    [XmlElement("PERSON_DATA")]
    //    public PersonData? PersonData { get; set; }

    //    [XmlElement("CARD_TYPE")]
    //    public string CardType { get; set; } = string.Empty;
    //}

    //public class ResponseStatus
    //{
    //    [XmlElement("CODE")]
    //    public string Code { get; set; } = string.Empty;

    //    [XmlElement("MESSAGE")]
    //    public string Message { get; set; } = string.Empty;
    //}

    //public class PersonData
    //{
    //    [XmlElement("NAME")]
    //    public string Name { get; set; } = string.Empty;

    //    [XmlElement("FATHER_HUSBAND_NAME")]
    //    public string FatherHusbandName { get; set; } = string.Empty;

    //    [XmlElement("PRESENT_ADDRESS")]
    //    public string PresentAddress { get; set; } = string.Empty;

    //    [XmlElement("PERMANANT_ADDRESS")]
    //    public string PermanantAddress { get; set; } = string.Empty;

    //    [XmlElement("DATE_OF_BIRTH")]
    //    public string DateOfBirth { get; set; } = string.Empty;

    //    [XmlElement("BIRTH_PLACE")]
    //    public string BirthPlace { get; set; } = string.Empty;

    //    [XmlElement("MOTHER_NAME")]
    //    public string MotherName { get; set; } = string.Empty;

    //    [XmlElement("PHOTOGRAPH")]
    //    public string Photograph { get; set; } = string.Empty;

    //    [XmlElement("EXPIRY_DATE")]
    //    public string ExpiryDate { get; set; } = string.Empty;
    //}

}
