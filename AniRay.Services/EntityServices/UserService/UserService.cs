using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Migrations;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using AniRay.Services.HelperServices.CurrentUserService;
using AniRay.Services.HelperServices.OtherHelpers;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Threading;
using System.Threading.Tasks;
using static AniRay.Services.HelperServices.OtherHelpers.CoreData;

namespace AniRay.Services.EntityServices.UserService
{
    public class UserService :
        BaseCRUDService<UserUM, UserEM, UserUSO, UserESO, User, UserUIR, UserEIR, UserUUR, UserEUR>, IUserService
    {
        private readonly ICurrentUserService _currentUser;

        public UserService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base(context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }

        

    }
}
