using Banking.FanFinancing.Shared.Enums;
using Banking.FanFinancing.Shared.Exceptions;
using Banking.FanFinancing.Shared.Helpers;
using Banking.FanFinancing.Shared.Models;
using Dapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Data;
using System.Text;

namespace Banking.FanFinancing.Shared.Filters
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateAuthToken : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string _connectionstring;

        public ValidateAuthToken(IOptions<DatabaseConnection> dbCon)
        {
            _connectionstring = dbCon.Value.AlBarakaAppConnectionString ?? throw new Exception($"No connection found.");
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            var authorizationToken = context.HttpContext.Request.Headers.TryGetValue("AuthToken", out var AuthHeader) ? AuthHeader.ToString() ?? "" : "";
            var channelId = context.HttpContext.Request.Headers.TryGetValue("ChannelID", out var id) ? id.ToString() ?? "" : "";
            var UserID = context.HttpContext.Request.Headers.TryGetValue("UserId", out var AuthUser) ? AuthUser.ToString() ?? "" : "";
            var Password = context.HttpContext.Request.Headers.TryGetValue("Password", out var AuthPassword) ? AuthPassword.ToString() ?? "" : "";
            if (user?.Identity?.IsAuthenticated == false || string.IsNullOrEmpty(authorizationToken))
            {
                throw new UnAuthorizeException(ResponseCodesEnum.A4401.GetEnumDescription());
            }
            var CustomIdentity = user is not null && user.Identity is not null ? (CustomIdentity)user.Identity : new CustomIdentity(new TokenClaims());
            if (CustomIdentity is null)
            {
                throw new UnAuthorizeException(ResponseCodesEnum.A4405.GetEnumDescription());
            }
            if (!UserID.Equals(CustomIdentity.UserName) || !Password.Equals(CustomIdentity.Password))
            {
                throw new UnAuthorizeException(ResponseCodesEnum.A4401.GetEnumDescription());
            }
            if (string.IsNullOrEmpty(channelId) || !channelId.ToLower().Equals("al baraka bank"))
            {
                TraceLogger.Log("AuthenticationAttribute", context.HttpContext);
                throw new CustomException("03", "Session Id (Header) is missing or invalid");
            }

            using var db = new SqlConnection(_connectionstring);
            var userClaims = await db.QueryFirstOrDefaultAsync<GenericResponse>("usp_validate_token_and_creds", new
            {
                UserName = UserID,
                Password = Password,
                Token = authorizationToken

            }, commandType: CommandType.StoredProcedure);


            if (userClaims is null || !userClaims.ResponseCode.Equals("00"))
            {
                if (userClaims is null)
                {
                    TraceLogger.Log("AuthenticationAttribute", context.HttpContext);
                    throw new UnAuthorizeException(ResponseCodesEnum.A4405.GetEnumDescription());

                }
                else
                {
                    TraceLogger.Log("AuthenticationAttribute", context.HttpContext);
                    throw new CustomException(userClaims.ResponseCode, userClaims.ResponseDescription);
                }
            }


        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateAuthTokenSession : Attribute, IAsyncAuthorizationFilter
    {
        private readonly string Connectionstring;
        string _cnic = "customerCNIC";
        string _mobile = "mobileNumber";
        string _transactionId = "transactionID";
        string _billConsumerNumber = "billConsumerNumber";
        public ValidateAuthTokenSession(IOptions<DatabaseConnection> dbCon, string transactionId = "transactionID", string propCnic = "customerCNIC", string propNumber = "mobileNumber", string billConsumerNumber = "billConsumerNumber")
        {
            Connectionstring = dbCon.Value.AlBarakaAppConnectionString ?? throw new Exception($"No connection found.");
            _transactionId = transactionId;
            _billConsumerNumber = billConsumerNumber;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            var authorizationToken = context.HttpContext.Request.Headers.TryGetValue("AuthToken", out var AuthHeader) ? AuthHeader.ToString() ?? "" : "";
            var SessionId = context.HttpContext.Request.Headers.TryGetValue("bankSessionId", out var id) ? id.ToString() ?? "" : "";
            var UserID = context.HttpContext.Request.Headers.TryGetValue("UserId", out var AuthUser) ? AuthUser.ToString() ?? "" : "";
            var Password = context.HttpContext.Request.Headers.TryGetValue("Password", out var AuthPassword) ? AuthPassword.ToString() ?? "" : "";
            var channelId = context.HttpContext.Request.Headers.TryGetValue("ChannelID", out var ChannelId) ? ChannelId.ToString() ?? "" : "";

            if (user?.Identity?.IsAuthenticated == false || string.IsNullOrEmpty(authorizationToken))
            {
                throw new UnAuthorizeException(ResponseCodesEnum.A4401.GetEnumDescription());
            }
            var CustomIdentity = user is not null && user.Identity is not null ? (CustomIdentity)user.Identity : new CustomIdentity(new TokenClaims());
            if (CustomIdentity is null)
            {
                throw new UnAuthorizeException(ResponseCodesEnum.A4405.GetEnumDescription());
            }
            if (!UserID.Equals(CustomIdentity.UserName) || !Password.Equals(CustomIdentity.Password))
            {
                throw new UnAuthorizeException(ResponseCodesEnum.A4401.GetEnumDescription());
            }
            if (string.IsNullOrEmpty(channelId) || !channelId.ToLower().Equals("al baraka bank"))
            {
                TraceLogger.Log("AuthenticationAttribute", context.HttpContext);
                throw new CustomException("03", "Session Id (Header) is missing or invalid");
            }


            if (string.IsNullOrEmpty(channelId) || !channelId.ToLower().Equals("al baraka bank"))
            {
                TraceLogger.Log("AuthenticationAttribute", context.HttpContext);
                throw new CustomException("03", "Session Id (Header) is missing or invalid");
            }
          
            

            string area = context.HttpContext.Request.Path.ToString();

            //if (!area.Contains("Loan") && !area.Equals("/RepaymentSchedule"))
            //{
            //    if (!CustomIdentity.SessionId.Equals(SessionId))
            //    {
            //        TraceLogger.Log("AuthenticationAttribute", context.HttpContext);
            //        throw new CustomException("03", "Session Id is required/invalid");
            //    }
            //}

            var helpingmethods = new HelpingMethod();

            var currentRequest = await helpingmethods.FormatRequestToString(context.HttpContext.Request);
            var jsonRequestProp = JObject.Parse(currentRequest)?.Properties() ?? Enumerable.Empty<JProperty>();

            string TransactionId = helpingmethods.ExtractKeyValueFromJson(jsonRequestProp, _transactionId, "transactionId");
            string BillConsumerNumber = helpingmethods.ExtractKeyValueFromJson(jsonRequestProp, _billConsumerNumber, "billConsumerNumber");
            string Cnic = string.Empty;
            string Mobile = string.Empty;
            if (string.IsNullOrEmpty(TransactionId))
            {
                Cnic = helpingmethods.ExtractKeyValueFromJson(jsonRequestProp, _cnic, "customerCNIC");
                Mobile = helpingmethods.ExtractKeyValueFromJson(jsonRequestProp, _mobile, "mobileNumber");
            }
            using var db = new SqlConnection(Connectionstring);
            if (area.Contains("CustomerOnboarding"))
            {
                area = "CustomerOnboarding";
            }
            else if (area.Contains("Generate") || area.Contains("Verify"))
            {
                area = "Otp";
            }
            else if (area.Contains("Disbursement"))
            {
                area = "Disbursement";
            }
            else if (area.Contains("Loan") || area.Equals("/RepaymentSchedule"))
            {
                area = "Loan";
            }
            else
            {
                area = string.Empty;
            }
            
            var userClaims = await db.QueryFirstOrDefaultAsync<GenericResponse>("usp_validate_session", new
            {
                TransactionId = TransactionId,
                BillReferenceNo = BillConsumerNumber,
                Cnic = Cnic,
                Mobile = Mobile,
                Session = SessionId,
                Token = authorizationToken,
                Endpoint = area

            }, commandType: CommandType.StoredProcedure);
            if (userClaims is null || !userClaims.ResponseCode.Equals("00"))
            {
                if (userClaims is null)
                {
                    TraceLogger.Log("AuthenticationAttribute", context.HttpContext);
                    throw new UnAuthorizeException(ResponseCodesEnum.A4405.GetEnumDescription());
                }
                else
                {
                    TraceLogger.Log("AuthenticationAttribute", context.HttpContext);
                    throw new CustomException(userClaims.ResponseCode, userClaims.ResponseDescription);
                }
            }


        }
    }

}

public class HelpingMethod
{
    public async Task<string> FormatRequestToString(HttpRequest request)
    {

        request.EnableBuffering();
        var body = request.Body;
        MemoryStream memoryStream = new MemoryStream();
        await body.CopyToAsync(memoryStream);
        var buffer = memoryStream.ToArray();
        await request.Body.ReadAsync(buffer, 0, buffer.Length);
        var bodyAsText = Encoding.UTF8.GetString(buffer);
        body.Position = 0;
        request.Body = body;
        return bodyAsText;
    }
    public string ExtractKeyValueFromJson(IEnumerable<JProperty> jsonProperties, params string[] keys)
    {
        try
        {
            if (keys == null || !keys.Any() || jsonProperties == null)
            {
                return "";
            }

            var matchingProperty = jsonProperties.FirstOrDefault(prop =>
                keys.Contains(prop.Name, StringComparer.OrdinalIgnoreCase));

            return matchingProperty?.Value?.ToString() ?? "";
        }
        catch (Exception)
        {
            return "";
        }
    }
}
