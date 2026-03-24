using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.AuthRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requestss.UserRequests;
using AniRay.Services.AuthentificationServices.TokenService;
using AniRay.Services.EntityServices.UserCartService;
using AniRay.Services.EntityServices.UserService;
using AniRay.Services.HelperServices.MailService;
using AniRay.Services.HelperServices.OtherHelpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Security.Claims;

namespace AniRay.Services.AuthentificationServices.AuthService
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly AniRayDbContext _context;
        private readonly IConfiguration _config;
        private readonly IMailService _mailService;
        private readonly IUserService _userService;
        private readonly IUserCartService _userCartService;
        private readonly int _accessTokenMinutes;
        private readonly int _refreshTokenDays;

        public AuthService(
            ITokenService tokenService,
            AniRayDbContext context,
            IConfiguration config,
            IMailService mailService,
            IUserService userService,
            IUserCartService userCartService)
        {
            _tokenService = tokenService;
            _context = context;
            _config = config;
            _mailService = mailService;
            _userService = userService;
            _userCartService = userCartService;
            _accessTokenMinutes = int.Parse(_config["Jwt:AccessTokenExpirationMinutes"]!);
            _refreshTokenDays = int.Parse(_config["Jwt:RefreshTokenExpirationDays"]!);
        }

        public async Task<ActionResult<object>> Register(UserIRU request, CancellationToken cancellationToken)
        {
            var resultUserInsert = await _userService.InsertEntityForUsers(request, cancellationToken);

            if (resultUserInsert.Result is not OkObjectResult okResult)
                return resultUserInsert.Result!;

            var createdUser = (UserMU)okResult.Value!;

            UserCartUIR cart = new() { UserId = createdUser.Id };
            var resultCartInsert = await _userCartService.InsertEntityForUsers(cart, cancellationToken);

            if (resultCartInsert.Result is not OkObjectResult)
                return resultCartInsert.Result!;

            await _mailService.SendEmailAsync(
                createdUser.Email,
                "Welcome to AniRay!",
                $"<h1>Welcome, {createdUser.Name}!</h1>" +
                "<p>Your account has been successfully created.</p>"
            );

            return new OkObjectResult(new
            {
                Message = "User registered successfully!",
                UserId = createdUser.Id,
                Email = createdUser.Email
            });
        }
        public async Task<ActionResult<AuthResult>> Login(LoginDto dto, CancellationToken cancellationToken)
        {
            var user = await _context.Users.AsNoTracking().Include(u => u.UserRole)
                .SingleOrDefaultAsync(u => u.Email == dto.Email, cancellationToken);
            if (user == null)
                return new UnauthorizedResult();

            var isPasswordValid = PasswordHelper.VerifyPassword(dto.Password, user.PasswordHash, user.PasswordSalt);
            if (!isPasswordValid)
                return new UnauthorizedResult();

            if (user.TwoFA)
                return await HandleTwoFactor(user, cancellationToken);

            return await GenerateAuthTokens(user, cancellationToken);
        }
        public async Task<ActionResult<AuthResult>> Verify2FA(Verify2FADto dto, CancellationToken cancellationToken)
        {
            var userCode = await _context.twoWayAuths.AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == dto.UserId, cancellationToken);

            if (userCode == null)
                return new UnauthorizedResult();

            var sentCode = TwoFactorAuthHelper.Hash2FA(dto.Code, userCode.CreatedAt);

            if (sentCode != userCode.Code)
            {
                userCode.Attempt++;

                if (userCode.Attempt >= 3)
                {
                    _context.twoWayAuths.Remove(userCode);
                    await _context.SaveChangesAsync(cancellationToken);
                    return new UnauthorizedResult();
                }

                _context.twoWayAuths.Update(userCode);
                await _context.SaveChangesAsync(cancellationToken);

                return new UnauthorizedResult();
            }

            _context.twoWayAuths.Remove(userCode);

            var user = await _context.Users
                .Include(u => u.UserRole)
                .FirstOrDefaultAsync(u => u.Id == dto.UserId, cancellationToken);

            if (user == null)
                return new UnauthorizedResult();

            return await GenerateAuthTokens(user, cancellationToken);
        }
        public async Task<ActionResult<AuthResult>> Refresh(RefreshRequestDto dto, CancellationToken cancellationToken)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.RefreshToken) || string.IsNullOrWhiteSpace(dto.AccessToken))
                return new BadRequestObjectResult("Invalid request.");

            var stored = await _context.RefreshTokens
                .SingleOrDefaultAsync(t => t.Token == dto.RefreshToken, cancellationToken);

            if (stored == null || stored.Revoked || stored.Expires < DateTime.UtcNow)
                return new UnauthorizedResult();

            var principal = _tokenService.GetPrincipalFromExpiredToken(dto.AccessToken);

            if (principal == null)
                return new UnauthorizedResult();

            var userIdFromAccessToken = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdFromAccessToken != stored.UserId.ToString())
                return new UnauthorizedResult();

            stored.Revoked = true;

            var newRefreshToken = _tokenService.CreateRefreshToken();
            stored.ReplacedByToken = newRefreshToken;

            var newRt = CreateRefreshToken(stored.UserId, newRefreshToken);

            _context.RefreshTokens.Add(newRt);
            _context.RefreshTokens.Update(stored);

            await _context.SaveChangesAsync(cancellationToken);

            var identity = new ClaimsIdentity(principal.Claims);

            var newAccessExpiry = DateTime.UtcNow.AddMinutes(_accessTokenMinutes);

            var newAccessToken = _tokenService.CreateAccessToken(identity, newAccessExpiry);

            return new OkObjectResult(
                new AuthResult(newAccessToken, newRefreshToken, newAccessExpiry)
            );
        }
        public async Task<ActionResult> Logout(LogoutDto dto, CancellationToken cancellationToken)
        {
            var token = await _context.RefreshTokens
                .SingleOrDefaultAsync(t => t.Token == dto.RefreshToken, cancellationToken);

            if (token == null)
                return new NotFoundObjectResult("Refresh token not found.");

            if (token.Revoked)
                return new BadRequestObjectResult("Refresh token already revoked.");

            token.Revoked = true;

            await _context.SaveChangesAsync(cancellationToken);

            return new OkResult();
        }

        #region Helpers
        private async Task<ActionResult<AuthResult>> HandleTwoFactor(User user, CancellationToken cancellationToken)
        {
            var existingRecord = await _context.twoWayAuths
                .FirstOrDefaultAsync(x => x.UserId == user.Id, cancellationToken);

            if (existingRecord != null)
                _context.twoWayAuths.Remove(existingRecord);

            string plainCode = TwoFactorAuthHelper.CodeGeneration();

            await _mailService.SendEmailAsync(
                user.Email,
                "Your 2FA Code",
                $"Your 2FA code is: <b>{plainCode}</b>"
            );

            var new2FA = TwoFactorAuthHelper.NewRecord(user.Id, plainCode);

            _context.twoWayAuths.Add(new2FA);
            await _context.SaveChangesAsync(cancellationToken);

            return new OkObjectResult(new
            {
                TwoFactorRequired = true,
                UserId = user.Id
            });
        }
        private async Task RevokeExistingRefreshTokens(int userId, CancellationToken cancellationToken)
        {
            var existingTokens = await _context.RefreshTokens
                .Where(x => x.UserId == userId && !x.Revoked && x.Expires > DateTime.UtcNow)
                .ToListAsync(cancellationToken);

            if (!existingTokens.Any())
                return;

            foreach (var token in existingTokens)
                token.Revoked = true;
        }
        private async Task<ActionResult<AuthResult>> GenerateAuthTokens(User user, CancellationToken cancellationToken)
        {
            var role = user.UserRole.Name;

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.Role, role)
            };

            var identity = new ClaimsIdentity(claims);

            var accessExpiry = DateTime.UtcNow.AddMinutes(_accessTokenMinutes);
            var accessToken = _tokenService.CreateAccessToken(identity, accessExpiry);
            var refreshToken = _tokenService.CreateRefreshToken();

            await RevokeExistingRefreshTokens(user.Id, cancellationToken);

            var rt = CreateRefreshToken(user.Id, refreshToken);
            _context.RefreshTokens.Add(rt);
            await _context.SaveChangesAsync(cancellationToken);

            return new OkObjectResult(new AuthResult(accessToken, refreshToken, accessExpiry));
        }
        private RefreshToken CreateRefreshToken(int userId, string token)
        {
            return new RefreshToken
            {
                Token = token,
                UserId = userId,
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddDays(_refreshTokenDays)
            };
        }
        #endregion

    }
}
