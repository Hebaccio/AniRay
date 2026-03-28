using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Entities;
using AniRay.Model.Requests.UserFavoritesRequests;
using AniRay.Services.EntityServices.UserFavoritesService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.EntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserFavoritesController :
        BaseCRUDController<UserFavoritesMU, UserFavoritesME, UserFavoritesSOU, UserFavoritesSOE, UserFavorites, UserFavoritesIRU, UserFavoritesIRE, UserFavoritesURU, UserFavoritesURE>
    {
        private readonly IUserFavoritesService _userFavoritesService;
        public UserFavoritesController(IUserFavoritesService Service) : base(Service)
        {
            _userFavoritesService = Service;
        }

        [Authorize(Roles = "User")]
        public override async Task<ActionResult<UserFavoritesMU>> InsertEntityForUsers(UserFavoritesIRU request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForUsers(request, cancellationToken);
        }

        [HttpDelete("RemoveFromFavorites/ForUsers")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<UserFavoritesMU>> RemoveMovieFromFavoritesForUsers(int id, CancellationToken cancellationToken)
        {
            return await _userFavoritesService.RemoveMovieFromFavorites(id, cancellationToken);
        }

        [HttpPut("UpdateEntity/ForUsers")]
        [Authorize(Roles = "User")]
        public new async Task<ActionResult<UserFavoritesMU>> UpdateEntityForUsers(UserFavoritesURU request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForUsers(request, cancellationToken);
        }

        [Authorize(Roles = "User")]
        public override async Task<ActionResult<PagedResult<UserFavoritesMU>>> GetPagedEntityForUsers([FromQuery] UserFavoritesSOU searchObject, CancellationToken cancellationToken)
        {
            return await base.GetPagedEntityForUsers(searchObject, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserFavoritesMU>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            return await base.EntityGetByIdForUsers(id, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserFavoritesME>> EntityGetByIdForEmployees(int id, CancellationToken cancellationToken)
        {
            return await base.EntityGetByIdForEmployees(id, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserFavoritesME>> InsertEntityForEmployees(UserFavoritesIRE request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForEmployees(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserFavoritesME>> UpdateEntityForEmployees(int id, UserFavoritesURE request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForEmployees(id, request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            return await base.SoftDelete(id, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserFavoritesMU>> UpdateEntityForUsers(int id, UserFavoritesURU request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForUsers(id, request, cancellationToken);
        }

    }
}