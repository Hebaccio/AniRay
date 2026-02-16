using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.EntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderController : BaseCRUDController<OrderModel, OrderSearchObject, OrderInsertRequest, OrderUpdateRequest>
    {
        public OrderController(IOrderService service)
            : base(service) { }

    }
}
