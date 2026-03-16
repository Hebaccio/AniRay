using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using AniRay.Services.HelperServices.CurrentUserService;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AniRay.Services.EntityServices.UserCartService
{
    public class UserCartService :
        BaseCRUDService<UserCartUM, UserCartEM, BaseSO, BaseSO, UserCart, UserCartUIR, UserCartEIR, UserCartUUR, UserCartEUR>, IUserCartService
    {
        private readonly ICurrentUserService _currentUser;
        public UserCartService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base (context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }

        

    }
}
