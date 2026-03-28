using AniRay.API.Controllers.BaseControllers;
using AniRay.Model.Entities;
using AniRay.Model.Requests.BluRayRequests;
using AniRay.Services.EntityServices.BluRayService;
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