using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Entities;
using AniRay.Model.Requests.BasicEntitiesRequests;
using AniRay.Services.EntityServices.OrderStatusService;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.BasicEntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderStatusController : BasicEntityController<OrderStatus>
    {
        protected new IOrderStatusService _service;
        public OrderStatusController(IOrderStatusService service)
            : base(service)
        {
            _service = service;
        }

        [HttpGet("GetPagedEntity/ForOrderStatusChange")]
        public async Task<ActionResult<Model.PagedResult<BaseClassMU>>> GetPagedEntity(CancellationToken cancellationToken)
        {
            return await _service.GetPagedEntity(cancellationToken);
        }

        [NonAction]
        public override Task<ActionResult<BaseClassME>> InsertEntityForEmployees(BaseClassIRE request, CancellationToken cancellationToken)
        {
            return base.InsertEntityForEmployees(request, cancellationToken);
        }

        [NonAction]
        public override Task<ActionResult<BaseClassME>> UpdateEntityForEmployees(int id, BaseClassURE request, CancellationToken cancellationToken)
        {
            return base.UpdateEntityForEmployees(id, request, cancellationToken);
        }

        [NonAction]
        public override Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            return base.SoftDelete(id, cancellationToken);
        }
    }
}