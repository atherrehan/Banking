using Banking.FanFinancing.Shared.Enums;
using Banking.FanFinancing.Shared.Helpers;

namespace Banking.FanFinancing.Infrastructure.Entities.Customer
{
    public class DataCheckRequestEntity
    {
        public string authKey { get; set; } = "Albaraka3472@Web24";
        public string memberCode { get; set; } = "16";
        public string controlBranchCode { get; set; } = "3472";
        public string subBranchCode { get; set; } = "0001";
        public string makerUserName { get; set; } = "mweb";
        public string makerPassword { get; set; } = "Dtam16@@";
        public string checkerUserName { get; set; } = "cweb";
        public string checkerPassword { get; set; } = "Dtac16@@";
        public string nicNoOrPassportNo { get; set; } = "3520272879277";
        public string cnicNo { get; set; } = "";
        public string p_date_time { get; set; } = DateTime.Now.ToString("hh:mm:ss dd-MM-yyyy");
        public string firstName { get; set; } = "SYED";
        public string middleName { get; set; } = "saleem";
        public string lastName { get; set; } = "Qureshi";
        public string dateOfBirth { get; set; } = "01/08/1989";
        public string gender { get; set; } = "M";
        public string fatherOrHusbandFirstName { get; set; } = "";
        public string fatherOrHusbandMiddleName { get; set; } = "";
        public string fatherOrHusbandLastName { get; set; } = "";
        public string address { get; set; } = "V7 input address";
        public string cityOrDistrict { get; set; } = "Lahore";
        public string phoneNo { get; set; } = "321456545";
        public string accountType { get; set; } = "BL";
        public string applicationId { get; set; } = AutoGenerate.Generate(GenerateEnum.Numeric, 5);
        public string amount { get; set; } = "1225400";
        public string associationType { get; set; } = "PRN";
        public string GroupId { get; set; } = "";
        public string TransactionNum { get; set; } = "";
        public string CO_NIC_1 { get; set; } = "";
        public string CO_NIC_2 { get; set; } = "";
        public string CO_NIC_3 { get; set; } = "";
        public string CO_NIC_4 { get; set; } = "";
        public string CO_NIC_5 { get; set; } = "";
        public string CO_CNIC_1 { get; set; } = "";
        public string CO_CNIC_2 { get; set; } = "";
        public string CO_CNIC_3 { get; set; } = "";
        public string CO_CNIC_4 { get; set; } = "";
        public string CO_CNIC_5 { get; set; } = "";
        public string CO_FIRST_NAME_1 { get; set; } = "";
        public string CO_FIRST_NAME_2 { get; set; } = "";
        public string CO_FIRST_NAME_3 { get; set; } = "";
        public string CO_FIRST_NAME_4 { get; set; } = "";
        public string CO_FIRST_NAME_5 { get; set; } = "";
        public string CO_MID_NAME_1 { get; set; } = "";
        public string CO_MID_NAME_2 { get; set; } = "";
        public string CO_MID_NAME_3 { get; set; } = "";
        public string CO_MID_NAME_4 { get; set; } = "";
        public string CO_MID_NAME_5 { get; set; } = "";
        public string CO_LAST_NAME_1 { get; set; } = "";
        public string CO_LAST_NAME_2 { get; set; } = "";
        public string CO_LAST_NAME_3 { get; set; } = "";
        public string CO_LAST_NAME_4 { get; set; } = "";
        public string CO_LAST_NAME_5 { get; set; } = "";
        public string CO_ASSOCIATION_1 { get; set; } = "";
        public string CO_ASSOCIATION_2 { get; set; } = "";
        public string CO_ASSOCIATION_3 { get; set; } = "";
        public string CO_ASSOCIATION_4 { get; set; } = "";
        public string CO_ASSOCIATION_5 { get; set; } = "";
        public string P_PDF_REQUIRED { get; set; } = "Y";

    }
    public class DataCheckResponseEntity
    {
        public string Status { get; set; } = string.Empty;
        public Report? Report { get; set; }
        public Pdfinbase64[]? PDFInBase64 { get; set; }
    }

    public class Report
    {
        public INDIVIDUAL_DETAIL[]? INDIVIDUAL_DETAIL { get; set; }
        public HOME_INFORMATION[]? HOME_INFORMATION { get; set; }
        public EMPLOYER_INFORMATION[]? EMPLOYER_INFORMATION { get; set; }
        public CREDIT_SCORE[]? CREDIT_SCORE { get; set; }
        public DEFAULT[]? DEFAULTS { get; set; }
        public FILE_NOTES[]? FILE_NOTES { get; set; }
        public CCP_MASTER[]? CCP_MASTER { get; set; }
        public CCP_DETAIL[]? CCP_DETAIL { get; set; }
        public CCP_SUMMARY[]? CCP_SUMMARY { get; set; }
        public CCP_SUMMARY_TOTAL[]? CCP_SUMMARY_TOTAL { get; set; }
        public Enquiry[]? ENQUIRIES { get; set; }
        public COLLATERAL[]? COLLATERAL { get; set; }
        public ASSOCIATION[]? ASSOCIATION { get; set; }
        public GUARANTEES_DETAILS[]? GUARANTEES_DETAILS { get; set; }
        public COBORROWER_DETAILS[]? COBORROWER_DETAILS { get; set; }
        public BANKRUPTCY_DETAILS[]? BANKRUPTCY_DETAILS { get; set; }
        public REVIEW[]? REVIEW { get; set; }
        public REPORT_MESSAGE[]? REPORT_MESSAGE { get; set; }
        public CREDIT_SUMMARY[]? CREDIT_SUMMARY { get; set; }
    }

    public class INDIVIDUAL_DETAIL
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string TRANX_DATE { get; set; } = string.Empty;
        public string CREATION_DATE { get; set; } = string.Empty;
        public string TITLE { get; set; } = string.Empty;
        public string FIRST_NAME { get; set; } = string.Empty;
        public string MIDDLE_NAME { get; set; } = string.Empty;
        public string LAST_NAME { get; set; } = string.Empty;
        public string NIC { get; set; } = string.Empty;
        public string CNIC { get; set; } = string.Empty;
        public string NTN { get; set; } = string.Empty;
        public string GENDER { get; set; } = string.Empty;
        public string DOB { get; set; } = string.Empty;
        public string MARITIAL_STATUS { get; set; } = string.Empty;
        public string DEPENDANTS { get; set; } = string.Empty;
        public string NATL_TYPE { get; set; } = string.Empty;
        public string NATIONALITY { get; set; } = string.Empty;
        public string QUALIFICATION { get; set; } = string.Empty;
        public string PROFESSION { get; set; } = string.Empty;
        public string MAKER { get; set; } = string.Empty;
        public string CHECKER { get; set; } = string.Empty;
        public string FATHER_HUSBAND_FNAME { get; set; } = string.Empty;
        public string FATHER_HUSBAND_MNAME { get; set; } = string.Empty;
        public string FATHER_HUSBAND_LNAME { get; set; } = string.Empty;
        public string IS_SELF { get; set; } = string.Empty;
        public string REFERENCE_NO { get; set; } = string.Empty;
        public string TRNX_RESULT { get; set; } = string.Empty;
    }

    public class HOME_INFORMATION
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string SEQ_NO { get; set; } = string.Empty;
        public string REPORTED_ON { get; set; } = string.Empty;
        public string ADDRESS { get; set; } = string.Empty;
        public string CITY { get; set; } = string.Empty;
        public string PERMANENT_ADDRESS { get; set; } = string.Empty;
        public string PERMANENT_CITY { get; set; } = string.Empty;
        public string PHONE1 { get; set; } = string.Empty;
        public string PHONE2 { get; set; } = string.Empty;
    }

    public class EMPLOYER_INFORMATION
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string SEQ_NO { get; set; } = string.Empty;
        public string REPORTED_ON { get; set; } = string.Empty;
        public string EMPLOYER { get; set; } = string.Empty;
        public string DESIGNATION { get; set; } = string.Empty;
        public string SELF_EMPLOYED { get; set; } = string.Empty;
        public string ADDRESS { get; set; } = string.Empty;
        public string CITY { get; set; } = string.Empty;
        public string PHONE1 { get; set; } = string.Empty;
        public string PHONE2 { get; set; } = string.Empty;
    }

    public class CREDIT_SCORE
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string SCORE { get; set; } = string.Empty;
        public string ODDS { get; set; } = string.Empty;
        public string PROB_OF_DEFALUT { get; set; } = string.Empty;
        public string PERCENTILE_RISK { get; set; } = string.Empty;
        public string SBP_RISK_LEVEL { get; set; } = string.Empty;
    }

    public class DEFAULT
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string LOAN_NO { get; set; } = string.Empty;
        public string NOTE { get; set; } = string.Empty;
        public string MEM_NAME { get; set; } = string.Empty;
        public string SUBBRN_NAME { get; set; } = string.Empty;
        public string REL_DT { get; set; } = string.Empty;
        public string ORG_STATUS_DATE { get; set; } = string.Empty;
        public string ORG_ACCT_NO { get; set; } = string.Empty;
        public string ORG_AMOUNT { get; set; } = string.Empty;
        public string ORG_ACCT_TY { get; set; } = string.Empty;
        public string ORG_MAPPED_ACCT_TY { get; set; } = string.Empty;
        public string ORG_STATUS { get; set; } = string.Empty;
        public string ORG_RTR { get; set; } = string.Empty;
        public string ORG_CURRENCY { get; set; } = string.Empty;
        public string UPD_STATUS_DATE { get; set; } = string.Empty;
        public string UPD_ACCT_NO { get; set; } = string.Empty;
        public string UPD_AMOUNT { get; set; } = string.Empty;
        public string UPD_ACCT_TY { get; set; } = string.Empty;
        public string UPD_MAPPED_ACCT_TY { get; set; } = string.Empty;
        public string UPD_STATUS { get; set; } = string.Empty;
        public string UPD_RTR { get; set; } = string.Empty;
        public string UPD_CURRENCY { get; set; } = string.Empty;
        public string SECURE { get; set; } = string.Empty;
        public string CLASS_CATG { get; set; } = string.Empty;
        public string SUB_OBJ { get; set; } = string.Empty;
        public string LOAN_CLASS_DESC { get; set; } = string.Empty;
        public string ASSOC_TY { get; set; } = string.Empty;
        public string GROUP_ID { get; set; } = string.Empty;
        public string RECOVERY_AMOUNT { get; set; } = string.Empty;
        public string RECOVERY_DATE { get; set; } = string.Empty;
        public string DISPUTE { get; set; } = string.Empty;
        public string LIMIT { get; set; } = string.Empty;
        public string OPEN_DATE { get; set; } = string.Empty;
        public string MATURITY_DATE { get; set; } = string.Empty;
        public string REPAYMENT_FREQ { get; set; } = string.Empty;
    }

    public class FILE_NOTES
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string REF_DATE { get; set; } = string.Empty;
        public string ACCT_NO { get; set; } = string.Empty;
        public string COMMENTS { get; set; } = string.Empty;
    }

    public class CCP_MASTER
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string LOAN_NO { get; set; } = string.Empty;
        public string NOTE { get; set; } = string.Empty;
        public string SEQ_NO { get; set; } = string.Empty;
        public string MEM_NAME { get; set; } = string.Empty;
        public string SUBBRN_NAME { get; set; } = string.Empty;
        public string ACCT_NO { get; set; } = string.Empty;
        public string ACCT_TY { get; set; } = string.Empty;
        public string MAPPED_ACCT_TY { get; set; } = string.Empty;
        public string TERM { get; set; } = string.Empty;
        public string ACCT_STATUS { get; set; } = string.Empty;
        public string LIMIT { get; set; } = string.Empty;
        public string OPEN_DATE { get; set; } = string.Empty;
        public string MATURITY_DATE { get; set; } = string.Empty;
        public string ASSOC_TY { get; set; } = string.Empty;
        public string GROUP_ID { get; set; } = string.Empty;
        public string STATUS_DATE { get; set; } = string.Empty;
        public string MIN_AMT_DUE { get; set; } = string.Empty;
        public string BNC_CHQ { get; set; } = string.Empty;
        public string LAST_PAYMENT { get; set; } = string.Empty;
        public string HIGH_CREDIT { get; set; } = string.Empty;
        public string OVERDUEAMOUNT { get; set; } = string.Empty;
        public string BALANCE { get; set; } = string.Empty;
        public string PAYMENT_STATUS { get; set; } = string.Empty;
        public string CURRENCY { get; set; } = string.Empty;
        public string RELATION_DT { get; set; } = string.Empty;
        public string SECURE { get; set; } = string.Empty;
        public string REPAYMENT_FREQ { get; set; } = string.Empty;
        public string CLASS_CATG { get; set; } = string.Empty;
        public string SUB_OBJ { get; set; } = string.Empty;
        public string LOAN_CLASS_DESC { get; set; } = string.Empty;
        public string RESCHEDULE_FLAG { get; set; } = string.Empty;
        public string RESCHEDULE_DT { get; set; } = string.Empty;
        public string DISPUTE { get; set; } = string.Empty;
    }

    public class CCP_DETAIL
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string SEQ_NO { get; set; } = string.Empty;
        public string STATUS_MONTH { get; set; } = string.Empty;
        public string PAYMENT_STATUS { get; set; } = string.Empty;
        public string OVERDUEAMOUNT { get; set; } = string.Empty;
    }

    public class CCP_SUMMARY
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string LOAN_NO { get; set; } = string.Empty;
        public string SEQ_NO { get; set; } = string.Empty;
        public string OK { get; set; } = string.Empty;
        public string X { get; set; } = string.Empty;
        public string P30 { get; set; } = string.Empty;
        public string P60 { get; set; } = string.Empty;
        public string P90 { get; set; } = string.Empty;
        public string P120 { get; set; } = string.Empty;
        public string P150 { get; set; } = string.Empty;
        public string P180 { get; set; } = string.Empty;
        public string LOSS { get; set; } = string.Empty;
    }

    public class CCP_SUMMARY_TOTAL
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string OK { get; set; } = string.Empty;
        public string X { get; set; } = string.Empty;
        public string P30 { get; set; } = string.Empty;
        public string P60 { get; set; } = string.Empty;
        public string P90 { get; set; } = string.Empty;
        public string P120 { get; set; } = string.Empty;
        public string P150 { get; set; } = string.Empty;
        public string P180 { get; set; } = string.Empty;
        public string LOSS { get; set; } = string.Empty;
    }

    public class Enquiry
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string MEM_NAME { get; set; } = string.Empty;
        public string SUBBRN_NAME { get; set; } = string.Empty;
        public string REFERENCE_DATE { get; set; } = string.Empty;
        public string SEPARATE_DATE { get; set; } = string.Empty;
        public string REFERENCE_NO { get; set; } = string.Empty;
        public string AMOUNT { get; set; } = string.Empty;
        public string ACCT_TY { get; set; } = string.Empty;
        public string MAPPED_ACCT_TY { get; set; } = string.Empty;
        public string ASSOC_TY { get; set; } = string.Empty;
        public string ENQ_STS { get; set; } = string.Empty;
        public string APPLICATION_DATE { get; set; } = string.Empty;
        public string GROUP_ID { get; set; } = string.Empty;
        public string CURRENCY { get; set; } = string.Empty;
        public string DISPUTE { get; set; } = string.Empty;
    }

    public class COLLATERAL
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string COLL_TYPE { get; set; } = string.Empty;
        public string AMOUNT { get; set; } = string.Empty;
        public string SECTION { get; set; } = string.Empty;
        public string ACCT_SEQ_NO { get; set; } = string.Empty;
    }

    public class ASSOCIATION
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string ASSOC { get; set; } = string.Empty;
        public string NIC_NO { get; set; } = string.Empty;
        public string CNIC_NO { get; set; } = string.Empty;
        public string FIRST_NAME { get; set; } = string.Empty;
        public string MIDDLE_NAME { get; set; } = string.Empty;
        public string LAST_NAME { get; set; } = string.Empty;
        public string GUARANTEE_AMOUNT { get; set; } = string.Empty;
        public string GUARANTEE_DATE { get; set; } = string.Empty;
        public string INVOCATION_DATE { get; set; } = string.Empty;
        public string SECTION { get; set; } = string.Empty;
        public string ACCT_SEQ_NO { get; set; } = string.Empty;
    }

    public class GUARANTEES_DETAILS
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string GRN_IN_FAVR { get; set; } = string.Empty;
        public string CNIC { get; set; } = string.Empty;
        public string AMOUNT { get; set; } = string.Empty;
        public string GRN_FILE_NO { get; set; } = string.Empty;
    }

    public class COBORROWER_DETAILS
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string OTHR_BWR { get; set; } = string.Empty;
        public string CNIC { get; set; } = string.Empty;
        public string CBR_FILE_NO { get; set; } = string.Empty;
    }

    public class BANKRUPTCY_DETAILS
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string COUT_NAME { get; set; } = string.Empty;
        public string DECL_DT { get; set; } = string.Empty;
    }

    public class REVIEW
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string MEMBER { get; set; } = string.Empty;
        public string REVIEWS { get; set; } = string.Empty;
    }

    public class REPORT_MESSAGE
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string MESSAGE { get; set; } = string.Empty;
    }

    public class CREDIT_SUMMARY
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRNX_NO { get; set; } = string.Empty;
        public string CATEGORY { get; set; } = string.Empty;
        public string NAME { get; set; } = string.Empty;
        public string CNIC { get; set; } = string.Empty;
        public string FILE_CREATION_DT { get; set; } = string.Empty;
        public string LOAN_COUNT { get; set; } = string.Empty;
        public string LOAN_LIMIT { get; set; } = string.Empty;
        public string LOAN_OS { get; set; } = string.Empty;
        public string LOAN_LESS_10K { get; set; } = string.Empty;
        public string LOAN_ABOVE_10K { get; set; } = string.Empty;
        public string CURRENT_30PLUS { get; set; } = string.Empty;
        public string CLOSE_WITHIN_MATURITY { get; set; } = string.Empty;
        public string CLOSE_AFTER_MATURITY { get; set; } = string.Empty;
        public string DEFAULT_COUNT { get; set; } = string.Empty;
        public string DEFAULT_OS { get; set; } = string.Empty;
        public string MONTH24_30PLUS { get; set; } = string.Empty;
        public string MONTH24_60PLUS { get; set; } = string.Empty;
        public string MONTH24_90PLUS { get; set; } = string.Empty;
        public string ENQUIRY_COUNT { get; set; } = string.Empty;
        public string ASSOCIATION { get; set; } = string.Empty;
    }

    public class Pdfinbase64
    {
        public string FILE_NO { get; set; } = string.Empty;
        public string TRANX_NO { get; set; } = string.Empty;
        public string pdfInBase64 { get; set; } = string.Empty;
    }



}
