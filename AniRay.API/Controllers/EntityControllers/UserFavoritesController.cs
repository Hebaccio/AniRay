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
        BaseCRUDController<UserFavoritesUM, UserFavoritesEM, UserFavoritesUSO, UserFavoritesESO, UserFavorites, UserFavoritesUIR, UserFavoritesEIR, UserFavoritesUUR, UserFavoritesEUR>
    {
        public UserFavoritesController(IUserFavoritesService Service) : base(Service) { }

        [Authorize(Roles = "User")]
        public override async Task<ActionResult<UserFavoritesUM>> InsertEntityForUsers(UserFavoritesUIR request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForUsers(request, cancellationToken);
        }

        [HttpPut("UpdateEntity/ForUsers")]
        [Authorize(Roles = "User")]
        public new async Task<ActionResult<UserFavoritesUM>> UpdateEntityForUsers(UserFavoritesUUR request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForUsers(request, cancellationToken);
        }

        [Authorize(Roles = "User")]
        public override async Task<ActionResult<PagedResult<UserFavoritesUM>>> GetPagedEntityForUsers([FromQuery] UserFavoritesUSO searchObject, CancellationToken cancellationToken)
        {
            return await base.GetPagedEntityForUsers(searchObject, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserFavoritesUM>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            return await base.EntityGetByIdForUsers(id, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserFavoritesEM>> EntityGetByIdForEmployees(int id, CancellationToken cancellationToken)
        {
            return await base.EntityGetByIdForEmployees(id, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserFavoritesEM>> InsertEntityForEmployees(UserFavoritesEIR request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForEmployees(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserFavoritesEM>> UpdateEntityForEmployees(int id, UserFavoritesEUR request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForEmployees(id, request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            return await base.SoftDelete(id, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserFavoritesUM>> UpdateEntityForUsers(int id, UserFavoritesUUR request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForUsers(id, request, cancellationToken);
        }
        
    }
}
