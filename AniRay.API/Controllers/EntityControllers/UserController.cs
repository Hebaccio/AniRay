using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Model.Requestss.UserRequests;
using AniRay.Services.EntityServices.UserService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Any;

namespace AniRay.API.Controllers.EntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : BaseCRUDController<UserMU, UserME, UserSOU, UserSOE, User, UserIRU, UserIRE, UserURU, UserURE>
    {

        public UserController(IUserService Service) : base(Service) { }

        [HttpGet("EntityGetById/ForUsers")]
        [Authorize(Roles = "User")]
        public new async Task<ActionResult<UserMU>> EntityGetByIdForUsers(CancellationToken cancellationToken)
        {
            return await base.EntityGetByIdForUsers(cancellationToken);
        }

        [HttpPatch("UpdateEntity/ForUsers")]
        [Authorize(Roles = "User")]
        public new async Task<ActionResult<UserMU>> UpdateEntityForUsers(UserURU request, CancellationToken cancellationToken)
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
        public override async Task<ActionResult<UserMU>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            return await base.EntityGetByIdForUsers(id, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<PagedResult<UserMU>>> GetPagedEntityForUsers([FromQuery] UserSOU searchObject, CancellationToken cancellationToken)
        {
            return await base.GetPagedEntityForUsers(searchObject, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserMU>> InsertEntityForUsers(UserIRU request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForUsers(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserME>> InsertEntityForEmployees(UserIRE request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForEmployees(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserMU>> UpdateEntityForUsers(int id, UserURU request, CancellationToken cancellationToken)
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