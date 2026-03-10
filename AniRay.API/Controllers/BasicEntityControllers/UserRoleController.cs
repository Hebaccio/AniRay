using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Entities;
using AniRay.Model.Migrations;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.EntityServices.UserRoleService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.BasicEntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserRoleController : BasicEntityController<UserRole>
    {
        public UserRoleController(IUserRoleService service)
            : base(service)
        {
        }

        [NonAction]
        public override async Task<ActionResult<PagedResult<BaseClassUM>>> GetPagedEntityForUsers([FromQuery] BaseClassUSO searchObject, CancellationToken cancellationToken)
        {
            return await base.GetPagedEntityForUsers(searchObject, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<PagedResult<BaseClassEM>>> GetPagedEntitiesForEmployees([FromQuery] BaseClassESO searchObject, CancellationToken cancellationToken)
        {
            return await base.GetPagedEntitiesForEmployees(searchObject, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<BaseClassEM>> InsertEntityForEmployees(BaseClassEIR request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForEmployees(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<BaseClassEM>> UpdateEntityForEmployees(int id, BaseClassEUR request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForEmployees(id, request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            return await base.SoftDelete(id, cancellationToken);
        }

    }
}