using AniRay.Model.Requests.AuthRequests;
using AniRay.Model.Requests.InsertRequests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.Services.AuthentificationServices
{
    public interface IAuthService
    {
        Task<ActionResult<object>> Register(UserUIR request, CancellationToken cancellationToken);
        Task<ActionResult<AuthResult>> Login(LoginDto dto, CancellationToken cancellationToken);
        Task<ActionResult<AuthResult>> Verify2FA(Verify2FADto dto, CancellationToken cancellationToken);
        Task<ActionResult<AuthResult>> Refresh(RefreshRequestDto dto, CancellationToken cancellationToken);
        Task<ActionResult> Logout(LogoutDto dto, CancellationToken cancellationToken);
    }
}
