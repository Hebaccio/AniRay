using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.AuthentificationServices.TokenService
{
    public interface ITokenService
    {
        string CreateAccessToken(ClaimsIdentity identity, DateTime expires);
        string CreateRefreshToken();
        ClaimsPrincipal? GetPrincipalFromExpiredToken(string token);
    }
}
