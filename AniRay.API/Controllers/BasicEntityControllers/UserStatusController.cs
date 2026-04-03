using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Entities;
using AniRay.Model.Requests.BasicEntitiesRequests;
using AniRay.Services.EntityServices.UserStatusService;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.BasicEntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserStatusController : BasicEntityController<UserStatus>
    {
        public UserStatusController(IUserStatusService service)
            : base(service)
        {
        }

        [NonAction]
        public override async Task<ActionResult<PagedResult<BaseClassMU>>> GetPagedEntityForUsers([FromQuery] BaseClassSOU searchObject, CancellationToken cancellationToken)
        {
            return await base.GetPagedEntityForUsers(searchObject, cancellationToken);
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