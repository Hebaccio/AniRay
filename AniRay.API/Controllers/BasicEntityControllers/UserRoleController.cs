using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Entities;
using AniRay.Model.Migrations;
using AniRay.Model.Requestss.BasicEntitiesRequests;
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
        public override async Task<ActionResult<PagedResult<BaseClassMU>>> GetPagedEntityForUsers([FromQuery] BaseClassSOU searchObject, CancellationToken cancellationToken)
        {
            return await base.GetPagedEntityForUsers(searchObject, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<PagedResult<BaseClassME>>> GetPagedEntitiesForEmployees([FromQuery] BaseClassSOE searchObject, CancellationToken cancellationToken)
        {
            return await base.GetPagedEntitiesForEmployees(searchObject, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<BaseClassME>> InsertEntityForEmployees(BaseClassIRE request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForEmployees(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<BaseClassME>> UpdateEntityForEmployees(int id, BaseClassURE request, CancellationToken cancellationToken)
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