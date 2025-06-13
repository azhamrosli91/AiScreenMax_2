using Base.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using MaxSys.Interface;

namespace MaxSys.Helpers
{
    public class JwtToken : IJWTToken
    {
        private readonly IConfiguration _configuration;
        public JwtToken(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public (bool success, string jsonObject, ClaimsIdentity claimsIdentity) JWTToken_Read(string JWTToken) 
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(JWTToken))
                {
                    return (false, string.Empty, null);
                }

                var jsonToken = handler.ReadToken(JWTToken) as JwtSecurityToken;

                if (jsonToken == null)
                {
                    return (false, string.Empty, null);
                }

                // Extract claims from the token
                var claims = jsonToken.Claims;
                var identity = new ClaimsIdentity(claims);

                return (true, jsonToken.Payload.SerializeToJson(), identity);
            }
            catch (Exception ex)
            {
                // Log the exception if necessary
                return (false, $"Error reading JWT Token: {ex.Message}", null);
            }
        }
        public (bool isSuccess, string TokenValue) JWTToken_Create(Claim[] ListofClaim)
        {
            bool success = false;
            var TokenValue = "";
            try
            {
                SymmetricSecurityKey jwtSecurityKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration.GetSection("JwtSettings").GetValue<string>("Key").ToString()));
                int expiredMinutes = _configuration.GetSection("JwtSettings").GetValue<int>("TokenExpiryInMinutes");
                string jwtTokenIssuer = _configuration.GetSection("JwtSettings").GetValue<string>("Issuer");
                string jwtTokenAudience = _configuration.GetSection("JwtSettings").GetValue<string>("Audience");

                var tokenHandler = new JwtSecurityTokenHandler();
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(ListofClaim),
                    Expires = DateTime.UtcNow.AddMinutes(expiredMinutes),
                    Issuer = jwtTokenIssuer,
                    Audience = jwtTokenAudience
                    //SigningCredentials = new SigningCredentials(jwtSecurityKey, SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                TokenValue = tokenHandler.WriteToken(token);

                success = true;
            }
            catch (Exception)
            {

                success = false;
            }


            return (success, TokenValue);

        }

    }
}
