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
    public class GenreController : BaseCRUDController<BaseClassUM, BaseClassEM, BaseClassUSO, BaseClassESO, Genre,
        BaseClassIR, BaseClassIR, BaseClassUUR, BaseClassEUR>
    {
        public GenreController(IGenreService service)
            : base(service)
        {
        }

        [Authorize(Policy = "Workers")]
        public override async Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            return await base.SoftDelete(id, cancellationToken);
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

    }
}