using Base.Model;
using System.Security.Claims;

namespace MaxSys.Interface
{
    public interface IJWTToken
    {
        (bool success, string jsonObject, ClaimsIdentity claimsIdentity) JWTToken_Read(string JWTToken);
        public (bool isSuccess, string TokenValue) JWTToken_Create(Claim[] ListofClaim);

    }
}
