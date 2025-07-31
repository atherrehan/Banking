using Banking.FanFinancing.Shared.Exceptions;
using Banking.FanFinancing.Shared.Helpers;
using Banking.FanFinancing.Shared.Models;
using Banking.FanFinancing.Shared.Services.Interface;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Banking.FanFinancing.Shared.Services
{
    public class AuthTokenService : IAuthTokenService
    {
        private readonly JwtSettings _jwtConfig;
        public AuthTokenService(IOptions<JwtSettings> jwtConfig)
        {
            _jwtConfig = jwtConfig.Value;
        }

        public string GenerateToken(TokenClaims claims)
        {
            if (claims is null)
            {
                return "";
            }
            var now = DateTime.UtcNow;
            var data = Security.EncryptAES(JsonConvert.SerializeObject(claims));
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Issuer = _jwtConfig.ValidIssuer,
                Audience = _jwtConfig.ValidAudience,
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim("Payload", data)
                    }),
                NotBefore = now,
                Expires = now.Add(TimeSpan.FromMinutes(_jwtConfig.Duration)),
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(Private(_jwtConfig.PrivateKey)), SecurityAlgorithms.RsaSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public bool ValidateToken(string token, out TokenClaims tokenClaims)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenKey = Encoding.ASCII.GetBytes(_jwtConfig.Secret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = _jwtConfig.ValidIssuer,
                    ValidateAudience = true,
                    ValidAudience = _jwtConfig.ValidAudience,
                    ClockSkew = TimeSpan.Zero,  // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new RsaSecurityKey(Public(_jwtConfig.PublicKey))
                }, out SecurityToken validatedToken);
                var jwtToken = (JwtSecurityToken)validatedToken;

                string Payload = Convert.ToString(jwtToken.Claims.First(x => x.Type == "Payload").Value);
                tokenClaims = JsonConvert.DeserializeObject<TokenClaims>(Security.DecryptAES(Payload)) ?? new TokenClaims();
                return true;
            }
            catch (SecurityTokenValidationException)
            {
                tokenClaims = new TokenClaims();
                return false;
            }
            catch (Exception)
            {
                throw new InvalidTokenException();
            }
        }

        private static RSA Private(string key)
        {
            var privateKey = RSA.Create(2048);
            privateKey.ImportRSAPrivateKey(Convert.FromBase64String(key), out _);
            return privateKey;
        }

        public static RSA Public(string key)
        {
            var publicKey = RSA.Create(2048);
            publicKey.ImportRSAPublicKey(Convert.FromBase64String(key), out _);
            return publicKey;
        }
    }
}
