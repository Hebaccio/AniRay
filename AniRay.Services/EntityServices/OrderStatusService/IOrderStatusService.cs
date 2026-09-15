using AniRay.Model.Entities;
using AniRay.Model.Requests.BasicEntitiesRequests;
using AniRay.Services.BaseServices.BasicEntitiesService;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.Services.EntityServices.OrderStatusService
{
    public interface IOrderStatusService : IBasicEntitiesService<OrderStatus>
    {
        public Task<ActionResult<Model.PagedResult<BaseClassMU>>> GetPagedEntity(CancellationToken cancellationToken);
    }
}
