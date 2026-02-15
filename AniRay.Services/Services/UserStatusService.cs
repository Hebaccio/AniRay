using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Services.Interfaces;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.Services
{
    public class UserStatusService : BasicEntitiesService<UserStatus>, IUserStatusService
    {
        public UserStatusService(AniRayDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }
    }
}
