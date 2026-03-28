using AniRay.Model.Requests.OrderRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.EntityServices.OrderService
{
    public interface IOrderService : 
        ICRUDService<OrderMU, OrderME, OrderSOU, OrderSOE, OrderIRU, OrderIRE, OrderURU, OrderURE>
    {

    }
}
