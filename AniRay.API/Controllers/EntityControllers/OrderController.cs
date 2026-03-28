using AniRay.API.Controllers.BaseControllers;
using AniRay.Model.Entities;
using AniRay.Model.Requests.OrderRequests;
using AniRay.Services.EntityServices.OrderService;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.EntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : BaseCRUDController<OrderMU, OrderME, OrderSOU, OrderSOE, Order, OrderIRU, OrderIRE, OrderURU, OrderURE>
    {
        public OrderController(IOrderService Service) : base(Service) { }

        [NonAction]
        public override async Task<ActionResult<OrderME>> InsertEntityForEmployees(OrderIRE request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForEmployees(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<OrderMU>> UpdateEntityForUsers(int id, OrderURU request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForUsers(id, request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            return await base.SoftDelete(id, cancellationToken);
        }

    }
}