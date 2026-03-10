using AniRay.API.Controllers.BaseControllers;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.AuthRequests;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Helpers;
using AniRay.Services.Interfaces;
using AniRay.Services.Services;
using AniRay.Services.Services.AuthentificationServices;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Dynamic.Core;
using System.Security.Claims;
using static Org.BouncyCastle.Crypto.Engines.SM2Engine;

namespace AniRay.API.Controllers.EntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Register")]
        public async Task<ActionResult<object>> Register(UserUIR request, CancellationToken ct)
        {
            return await _authService.Register(request, ct);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AuthResult>> Login(LoginDto dto, CancellationToken ct)
        {
            return await _authService.Login(dto, ct);
        }

        [HttpPost("Verify-2FA")]
        public async Task<ActionResult<AuthResult>> Verify2FA(Verify2FADto dto, CancellationToken ct)
        {
            return await _authService.Verify2FA(dto, ct);
        }

        [HttpPost("Refresh")]
        public async Task<ActionResult<AuthResult>> Refresh(RefreshRequestDto dto, CancellationToken ct)
        {
            return await _authService.Refresh(dto, ct);
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(LogoutDto dto, CancellationToken ct)
        {
            return await _authService.Logout(dto, ct);
        }
    }
}
