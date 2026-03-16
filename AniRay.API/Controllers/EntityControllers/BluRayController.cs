using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Model.Requestss.BluRay;
using AniRay.Services.EntityServices.BluRayService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.EntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class BluRayController : BaseCRUDController<BluRayMU, BluRayME, BluRaySOU, BluRaySOE, BluRay, BluRayIRU, BluRayIRE, BluRayURU, BluRayURE>
    {
        public BluRayController(IBluRayService Service) : base(Service) { }

        [NonAction]
        public override async Task<ActionResult<BluRayMU>> InsertEntityForUsers(BluRayIRU request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForUsers(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<BluRayMU>> UpdateEntityForUsers(int id, BluRayURU request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForUsers(id, request, cancellationToken);
        }
    }
}