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
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AniRay.Services.EntityServices.UserFavoritesService
{
    public class UserFavoritesService :
        BaseCRUDService<UserFavoritesUM, UserFavoritesEM, UserFavoritesUSO, UserFavoritesESO, UserFavorites, UserFavoritesUIR, UserFavoritesEIR, UserFavoritesUUR, UserFavoritesEUR>,
        IUserFavoritesService
    {
        private readonly ICurrentUserService _currentUser;
        public UserFavoritesService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base(context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }

        

    }
}
