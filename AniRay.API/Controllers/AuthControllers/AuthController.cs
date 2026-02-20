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
        private readonly ITokenService _tokenService;
        private readonly AniRayDbContext _context;
        private readonly IConfiguration _config;
        private readonly IMailService _mailService;
        private readonly IUserService _userService;

        public AuthController(ITokenService tokenService, AniRayDbContext context, IConfiguration config, IMailService mailService, IUserService userService)
        {
            _tokenService = tokenService;
            _context = context;
            _config = config;
            _mailService = mailService;
            _userService = userService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] UserIR request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _userService.InsertEntityForUsers(request);

            if (!result.Success)
                return BadRequest(result.Message);

            var createdUser = result.Data;

            await _mailService.SendEmailAsync(
                    createdUser.Email,
                    "Welcome to AniRay!",
                    $"<h1>Welcome, {createdUser.Name}!</h1>" +
                    "<p>Your account has been successfully created.</p>");

            return Ok(new
            {
                Message = "User registered successfully!",
                UserId = createdUser.Id,
                Email = createdUser.Email
            });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _context.Users.Where(u => u.Email == dto.Email).Include(u => u.UserRole).FirstOrDefaultAsync();
            if (user == null) return Unauthorized();

            var isPasswordValid = PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt);
            if(!isPasswordValid) return Unauthorized("Invalid email or password.");

            if (user.TwoFA)
            {
                var existingRecord = await _context.twoWayAuths.FirstOrDefaultAsync(x => x.UserId == user.Id);
                if (existingRecord != null) _context.twoWayAuths.Remove(existingRecord);

                string plainCode = TwoFactorAuthHelper.CodeGeneration();

                await _mailService.SendEmailAsync(
                    user.Email,
                    "Your 2FA Code",
                    $"Your 2FA code is: <b>{plainCode}</b>"
                );

                var new2FA = TwoFactorAuthHelper.NewRecord(user.Id, plainCode);
                _context.twoWayAuths.Add(new2FA);
                await _context.SaveChangesAsync();

                return Ok(new { TwoFactorRequired = true, UserId = user.Id });
            }

            var role = user.UserRole.Name;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims);
            var accessExpiry = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:AccessTokenExpirationMinutes"]!));
            var accessToken = _tokenService.CreateAccessToken(identity, accessExpiry);

            var refreshToken = _tokenService.CreateRefreshToken();
            var rt = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenExpirationDays"]!))
            };

            _context.RefreshTokens.Add(rt);
            await _context.SaveChangesAsync();

            return Ok(new AuthResult(accessToken, refreshToken, accessExpiry));
        }

        [HttpPost("Verify-2FA")]
        public async Task<IActionResult> Verify2FA([FromBody] Verify2FADto dto)
        {
            var UserCode = await _context.twoWayAuths.FirstOrDefaultAsync(x => x.UserId == dto.UserId);
            var SentCode = TwoFactorAuthHelper.Hash2FA(dto.Code, UserCode.CreatedAt);

            if (SentCode != UserCode.Code)
                return Unauthorized("Invalid 2FA code.");

            _context.twoWayAuths.Remove(UserCode);
            await _context.SaveChangesAsync();

            var user = await _context.Users.Where(u=> u.Id == dto.UserId).Include(u=> u.UserRole).FirstOrDefaultAsync();
            var role = user.UserRole.Name;

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims);
            var accessExpiry = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:AccessTokenExpirationMinutes"]!));
            var accessToken = _tokenService.CreateAccessToken(identity, accessExpiry);

            var refreshToken = _tokenService.CreateRefreshToken();
            var rt = new RefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenExpirationDays"]!))
            };

            _context.Add(rt);
            await _context.SaveChangesAsync();

            return Ok(new AuthResult(accessToken, refreshToken, accessExpiry));
        }

        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshRequestDto dto)
        {
            var stored = await _context.RefreshTokens.SingleOrDefaultAsync(t => t.Token == dto.RefreshToken);
            if (stored == null || stored.Revoked || stored.Expires < DateTime.UtcNow)
                return Unauthorized("Invalid refresh token");

            var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);
            if (principal == null) return Unauthorized();

            stored.Revoked = true;
            var newRefresh = _tokenService.CreateRefreshToken();
            stored.ReplacedByToken = newRefresh;

            var newRt = new RefreshToken
            {
                Token = newRefresh,
                UserId = stored.UserId,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(int.Parse(_config["Jwt:RefreshTokenExpirationDays"]!))
            };

            _context.RefreshTokens.Add(newRt);
            _context.RefreshTokens.Update(stored);
            await _context.SaveChangesAsync();

            var identity = new ClaimsIdentity(principal.Claims);
            var newAccessExpiry = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:AccessTokenExpirationMinutes"]!));
            var newAccessToken = _tokenService.CreateAccessToken(identity, newAccessExpiry);

            return Ok(new AuthResult(newAccessToken, newRefresh, newAccessExpiry));
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogoutDto dto)
        {
            var token = await _context.RefreshTokens.SingleOrDefaultAsync(t => t.Token == dto.RefreshToken);
            if (token == null)
            {
                return NotFound("Refresh token not found.");
            }

            if (token.Revoked)
            {
                return BadRequest("Refresh token already revoked.");
            }

            token.Revoked = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
