using AniRay.Model.Requests.BasicEntitiesRequests;
using AniRay.Services.BaseServices.BasicEntitiesService;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.BaseControllers
{
    [ApiController]
    [Route("[controller]")]
    public class BasicEntityController<TdbEntity> : BaseCRUDController<BaseClassMU, BaseClassME, BaseClassSOU, BaseClassSOE, TdbEntity,
        BaseClassIRU, BaseClassIRE, BaseClassURU, BaseClassURE> where TdbEntity : class
    {
        public BasicEntityController(IBasicEntitiesService<TdbEntity> service)
            : base(service)
        {
        }

        [NonAction]
        public override async Task<ActionResult<BaseClassMU>> InsertEntityForUsers(BaseClassIRU request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForUsers(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<BaseClassMU>> UpdateEntityForUsers(int id, BaseClassURU request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForUsers(id, request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<BaseClassMU>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            return await base.EntityGetByIdForUsers(id, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<BaseClassME>> EntityGetByIdForEmployees(int id, CancellationToken cancellationToken)
        {
            return await base.EntityGetByIdForEmployees(id, cancellationToken);
        }

    }
}