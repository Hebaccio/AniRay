using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Interfaces;
using AniRay.Services.Services.AuthentificationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.EntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserFavoritesController : 
        BaseCRUDController<UserFavoritesM, UserFavoritesM, UserFavoritesSO, UserFavoritesSO, UserFavorites, UserFavoritesIR, UserFavoritesIR, UserFavoritesUR, UserFavoritesUR>
    {
        public UserFavoritesController(IUserFavoritesService Service) : base(Service) { }


        [Authorize(Roles = "User")]
        public override async Task<ActionResult<PagedResult<UserFavoritesM>>> GetPagedEntityForUsers([FromQuery] UserFavoritesSO searchObject, CancellationToken cancellationToken)
        {
            return await base.GetPagedEntityForUsers(searchObject, cancellationToken);
        }

        [NonAction]
        public override Task<ActionResult<UserFavoritesM>> EntityGetByIdForEmployees(int id, CancellationToken cancellationToken)
        {
            return base.EntityGetByIdForEmployees(id, cancellationToken);
        }

        [NonAction]
        public override Task<ActionResult<UserFavoritesM>> InsertEntityForEmployees(UserFavoritesIR request, CancellationToken cancellationToken)
        {
            return base.InsertEntityForEmployees(request, cancellationToken);
        }

        [NonAction]
        public override Task<ActionResult<UserFavoritesM>> UpdateEntityForEmployees(int id, UserFavoritesUR request, CancellationToken cancellationToken)
        {
            return base.UpdateEntityForEmployees(id, request, cancellationToken);
        }

        [NonAction]
        public override Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            return base.SoftDelete(id, cancellationToken);
        }

        [NonAction]
        public override Task<ActionResult<UserFavoritesM>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            return base.EntityGetByIdForUsers(id, cancellationToken);
        }
    }
}
