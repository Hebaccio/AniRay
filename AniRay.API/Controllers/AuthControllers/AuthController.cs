using AniRay.Model.AuthRequests;
using AniRay.Model.Requests.UserRequests;
using AniRay.Services.AuthentificationServices.AuthService;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<object>> Register(UserIRU request, CancellationToken ct)
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
