using AniRay.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public int? UserId
        {
            get
            {
                var claim = User?.FindFirst(ClaimTypes.NameIdentifier);
                return claim != null && int.TryParse(claim.Value, out int id)
                    ? id
                    : null;
            }
        }

        public bool IsAuthenticated =>
            User?.Identity?.IsAuthenticated ?? false;

        public bool IsUser() =>
            User?.IsInRole("User") ?? false;

        public bool IsEmployee() =>
            User?.IsInRole("Employee") ?? false;

        public bool IsBoss() =>
            User?.IsInRole("Boss") ?? false;

        public bool IsWorker() =>
            IsEmployee() || IsBoss();

        public bool IsSelf(int userId) =>
            UserId.HasValue && UserId.Value == userId;
    }
}
