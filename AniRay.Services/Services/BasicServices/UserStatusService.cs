using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Services.Interfaces;
using AniRay.Services.Interfaces.BasicServices;
using AniRay.Services.Services.BaseServices;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.Services.BasicServices
{
    public class UserStatusService : BasicEntitiesService<UserStatus>, IUserStatusService
    {
        private readonly ICurrentUserService _currentUserService;
        public UserStatusService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUserService)
            : base(context, mapper, currentUserService)
        {
            _currentUserService = currentUserService;
        }
    }
}
