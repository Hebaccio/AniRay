using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.EntityServices.OrderService
{
    public interface IOrderService : 
        ICRUDService<OrderUM, OrderEM, OrderUSO, OrderESO, OrderUIR, OrderEIR, OrderUUR, OrderEUR>
    {

    }
}
