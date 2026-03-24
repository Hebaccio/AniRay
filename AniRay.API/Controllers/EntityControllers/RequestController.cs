using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Model.Requestss.RequestRequests;
using AniRay.Services.EntityServices.RequestService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.EntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class RequestController : BaseCRUDController<RequestMU, RequestME, RequestSOU, RequestSOE, Request, RequestIRU, RequestIRE, RequestURU, RequestURE>
    {
        public RequestController(IRequestService Service) : base(Service) { }

        [NonAction]
        public override async Task<ActionResult<RequestME>> InsertEntityForEmployees(RequestIRE request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForEmployees(request, cancellationToken);
        }

        [NonAction]
        public override Task<ActionResult<RequestMU>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            return base.EntityGetByIdForUsers(id, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            return await base.SoftDelete(id, cancellationToken);
        }

    }
}