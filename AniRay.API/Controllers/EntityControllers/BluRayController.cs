using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.EntityServices.BluRayService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.EntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class BluRayController : BaseCRUDController<BluRayUM, BluRayEM, BluRayUSO, BluRayESO, BluRay, BluRayUIR, BluRayEIR, BluRayUUR, BluRayEUR>
    {
        public BluRayController(IBluRayService Service) : base(Service) { }

        [NonAction]
        public override async Task<ActionResult<BluRayUM>> InsertEntityForUsers(BluRayUIR request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForUsers(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<BluRayUM>> UpdateEntityForUsers(int id, BluRayUUR request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForUsers(id, request, cancellationToken);
        }
    }
}
