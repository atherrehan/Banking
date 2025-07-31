using Banking.FanFinancing.Shared.Enums;
using Banking.FanFinancing.Shared.Exceptions;
using Banking.FanFinancing.Shared.Helpers;
using Banking.FanFinancing.Shared.Models;
using Banking.FanFinancing.Shared.Services.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using System.Security.Principal;


namespace Banking.FanFinancing.Shared.Middleware
{
    public class AuthMiddleware
    {
        private IGuidService? _gUIDService;
        private readonly RequestDelegate _next;
        public AuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context, IGuidService guidService, IAuthTokenService tokenService)
        {
            _gUIDService = guidService;
            var isAllowAnonymous = context.Features.Get<IEndpointFeature>()?.Endpoint?.Metadata.Any(x => x.GetType() == typeof(AllowAnonymousAttribute));
            if (!(isAllowAnonymous.HasValue && isAllowAnonymous.Value))
            {
                var token = context.Request.Headers.TryGetValue("authToken", out var AuthHeader) ? AuthHeader.ToString() ?? "" : "";
                if (string.IsNullOrEmpty(token) && !context.Request.Path.ToString().Contains("/GenerateAuth"))
                {
                    TraceLogger.Log("AuthMiddleware", context);
                    throw new UnAuthorizeException("Your application cannot be processed at the moment. Please try again later");

                }
                ValidateAndAttachUserToContext(context, token, tokenService);

            }
            context.Response.OnStarting(state =>
            {
                var httpContext = (HttpContext)state;
                if (context.User.Identity is not null)
                {
                    httpContext.Response.Headers.Append("X-journy", context.User.Identity.IsAuthenticated ? Convert.ToString(((CustomIdentity)context.User.Identity)?.JourneyId) ?? "0" : "0");

                }
                httpContext.Response.Headers.Append("X-GUID", _gUIDService.GUID() ?? "0");
                return Task.CompletedTask;
            }, context);

            await _next(context);
        }
        void ValidateAndAttachUserToContext(HttpContext context, string token, IAuthTokenService tokenService)
        {
            if (tokenService.ValidateToken(token, out TokenClaims tokenPayload))
            {
                var User = new GenericPrincipal(new CustomIdentity(tokenPayload), null);
                context.User = User;
                return;
            }
            TraceLogger.Log("AuthMiddleware", context);
            throw new UnAuthorizeException(ResponseCodesEnum.A4402.GetEnumDescription());
        }
    }
}
