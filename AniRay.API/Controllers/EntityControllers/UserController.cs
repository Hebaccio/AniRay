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
    public class UserController : BaseCRUDController<UserUM, UserEM, UserESO, UserESO, User, UserIR, UserIR, UserUUR, UserEUR>
    {

        public UserController(IUserService userService) : base(userService) { }

        [NonAction]
        public override async Task<ActionResult<UserEM>> InsertEntityForEmployees(UserIR request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForEmployees(request, cancellationToken);
        }

        [Authorize(Roles = "User")]
        public override async Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            return await base.SoftDelete(id, cancellationToken);
        }

        [Authorize(Roles = "User")]
        public override async Task<ActionResult<UserUM>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            return await base.EntityGetByIdForUsers(id, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<PagedResult<UserUM>>> GetPagedEntityForUsers([FromQuery] UserESO searchObject, CancellationToken cancellationToken)
        {
            return await base.GetPagedEntityForUsers(searchObject, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserUM>> InsertEntityForUsers(UserIR request, CancellationToken cancellationToken)
        {
            return await  base.InsertEntityForUsers(request, cancellationToken);
        }
    }
}
