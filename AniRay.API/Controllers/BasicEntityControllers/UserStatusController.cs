using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Entities;
using AniRay.Model.Migrations;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Interfaces.BasicServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.BasicEntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserStatusController : BaseCRUDController<BaseClassUM, BaseClassEM, BaseClassUSO, BaseClassESO, UserStatus,
        BaseClassIR, BaseClassIR, BaseClassUUR, BaseClassEUR>
    {
        public UserStatusController(IUserStatusService service)
            : base(service)
        {
        }

        [NonAction]
        public override async Task<ActionResult<BaseClassEM>> EntityGetByIdForEmployees(int id, CancellationToken cancellationToken)
        {
            return await base.EntityGetByIdForEmployees(id, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<BaseClassEM>> InsertEntityForEmployees(BaseClassIR request, CancellationToken cancellationToken)
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

        [NonAction]
        public override async Task<ActionResult<BaseClassUM>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            return await base.EntityGetByIdForUsers(id, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<BaseClassUM>> InsertEntityForUsers(BaseClassIR request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForUsers(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<BaseClassUM>> UpdateEntityForUsers(int id, BaseClassUUR request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForUsers(id, request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<PagedResult<BaseClassUM>>> GetPagedEntityForUsers([FromQuery] BaseClassUSO searchObject, CancellationToken cancellationToken)
        {
            return await base.GetPagedEntityForUsers(searchObject, cancellationToken);
        }

    }
}