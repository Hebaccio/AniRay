using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.EntityServices.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.EntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : BaseCRUDController<UserUM, UserEM, UserUSO, UserESO, User, UserUIR, UserEIR, UserUUR, UserEUR>
    {

        public UserController(IUserService Service) : base(Service) { }

        [HttpGet("EntityGetById/ForUsers")]
        [Authorize(Roles = "User")]
        public new async Task<ActionResult<UserUM>> EntityGetByIdForUsers(CancellationToken cancellationToken)
        {
            return await base.EntityGetByIdForUsers(cancellationToken);
        }

        [HttpPut("UpdateEntity/ForUsers")]
        [Authorize(Roles = "User")]
        public new async Task<ActionResult<UserUM>> UpdateEntityForUsers(UserUUR request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForUsers(request, cancellationToken);
        }

        [HttpDelete("SoftDelete")]
        [Authorize(Roles = "User")]
        public new async Task<ActionResult<string>> SoftDelete(CancellationToken cancellationToken)
        {
            return await base.SoftDelete(cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserUM>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            return await base.EntityGetByIdForUsers(id, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<PagedResult<UserUM>>> GetPagedEntityForUsers([FromQuery] UserUSO searchObject, CancellationToken cancellationToken)
        {
            return await base.GetPagedEntityForUsers(searchObject, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserUM>> InsertEntityForUsers(UserUIR request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForUsers(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserEM>> InsertEntityForEmployees(UserEIR request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForEmployees(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserUM>> UpdateEntityForUsers(int id, UserUUR request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForUsers(id, request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            return await base.SoftDelete(id, cancellationToken);
        }

    }
}
