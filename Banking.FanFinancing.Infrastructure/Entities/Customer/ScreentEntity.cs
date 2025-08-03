namespace Banking.FanFinancing.Infrastructure.Entities.Customer
{
    public class ScreeningRequestConfig
    {
        public string storeInput { get; set; } = string.Empty;
        public string responseType { get; set; } = string.Empty;
        public string entityDetails { get; set; } = string.Empty;
        public string showMatchedData { get; set; } = string.Empty;
    }

    public class ScreeningRequestInputRecord
    {
        public string fieldName { get; set; } = string.Empty;
        public string fieldValue { get; set; } = string.Empty;
    }

    public class ScreeningRequestEntity
    {
        public string profileId { get; set; } = string.Empty;
        public int accountId { get; set; }
        public int datasetId { get; set; }
        public string correlationId { get; set; } = string.Empty;
        public List<ScreeningRequestInputRecord>? inputRecord { get; set; }
        public ScreeningRequestConfig? config { get; set; }
    }

    public class ScreeningResponseEntity
    {
        public string responseType { get; set; } = string.Empty;
        public string screeningStatus { get; set; } = string.Empty;
        public string importStatus { get; set; } = string.Empty;
        public string correlationId { get; set; } = string.Empty;
        public string uniqueId { get; set; } = string.Empty;
        public int accountId { get; set; }
        public int datasetId { get; set; }
        public string datasetName { get; set; } = string.Empty;
        public int caseId { get; set; }
        public int totalSubCases { get; set; }
        public int totalMatches { get; set; }
        public List<ScreeningResponseSubCaseDetail>? subCaseDetails { get; set; }
    }

    public class ScreeningResponseSubCaseDetail
    {
        public string subcase { get; set; } = string.Empty;
        public string requiresReview { get; set; } = string.Empty;
        public string status { get; set; } = string.Empty;
        public string assignedUser { get; set; } = string.Empty;
        public int totalMatches { get; set; }
    }



}
