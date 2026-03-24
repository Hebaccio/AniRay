using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Entities;
using AniRay.Model.Migrations;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Model.Requestss.BasicEntitiesRequests;
using AniRay.Services.EntityServices.OrderStatusService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.BasicEntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderStatusController : BasicEntityController<OrderStatus>
    {
        public OrderStatusController(IOrderStatusService service)
            : base(service)
        {
        }

        [NonAction]
        public override async Task<ActionResult<PagedResult<BaseClassMU>>> GetPagedEntityForUsers([FromQuery] BaseClassSOU searchObject, CancellationToken cancellationToken)
        {
            return await base.GetPagedEntityForUsers(searchObject, cancellationToken);
        }

    }
}