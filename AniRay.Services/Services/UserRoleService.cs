using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Services.Interfaces;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.Services
{
    public class UserRoleService : BasicEntitiesService<UserRole>, IUserRoleService
    {
        public UserRoleService(AniRayDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }
    }

}
