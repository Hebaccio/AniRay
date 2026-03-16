using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using AniRay.Services.HelperServices.CurrentUserService;
using AniRay.Services.HelperServices.OtherHelpers;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using static AniRay.Services.HelperServices.OtherHelpers.CoreData;

namespace AniRay.Services.EntityServices.OrderService
{
    public class OrderService : 
        BaseCRUDService<OrderUM, OrderEM, OrderUSO, OrderESO, Order, OrderUIR, OrderEIR, OrderUUR, OrderEUR>, IOrderService
    {
        private readonly ICurrentUserService _currentUser;
        public OrderService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser)
            :base(context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }

        

    }
}
