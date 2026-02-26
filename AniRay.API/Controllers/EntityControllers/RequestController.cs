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
    public class RequestController : BaseCRUDController<RequestUM, RequestUM, RequestUSO, RequestESO, Request, RequestUIR, RequestUIR, RequestUUR, RequestUUR>
    {
        public RequestController(IRequestService Service) : base(Service) { }


        [NonAction]
        public override Task<ActionResult<RequestUM>> InsertEntityForEmployees(RequestUIR request, CancellationToken cancellationToken)
        {
            return base.InsertEntityForEmployees(request, cancellationToken);
        }

        [NonAction]
        public override Task<ActionResult<RequestUM>> UpdateEntityForEmployees(int id, RequestUUR request, CancellationToken cancellationToken)
        {
            return base.UpdateEntityForEmployees(id, request, cancellationToken);
        }

        [NonAction]
        public override Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            return base.SoftDelete(id, cancellationToken);
        }

        [Authorize(Roles = "User")]
        public override async Task<ActionResult<RequestUM>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            return await _service.EntityGetByIdForUsers(id, cancellationToken);
        }

        [Authorize (Roles = "User")]
        public override async Task<ActionResult<PagedResult<RequestUM>>> GetPagedEntityForUsers([FromQuery] RequestUSO searchObject, CancellationToken cancellationToken)
        {
            return await _service.GetPagedEntityForUsers(searchObject, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<RequestUM>> UpdateEntityForUsers(int id, RequestUUR request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForUsers(id, request, cancellationToken);
        }
    }
}
