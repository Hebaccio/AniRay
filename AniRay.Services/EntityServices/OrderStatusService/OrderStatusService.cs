using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.BasicEntitiesRequests;
using AniRay.Services.BaseServices.BasicEntitiesService;
using AniRay.Services.HelperServices.CurrentUserService;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.EntityServices.OrderStatusService
{
    public class OrderStatusService : BasicEntitiesService<OrderStatus>, IOrderStatusService
    {
        private readonly ICurrentUserService _currentUser;
        public OrderStatusService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser)
            : base(context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }

        public virtual async Task<ActionResult<Model.PagedResult<BaseClassMU>>> GetPagedEntity(CancellationToken cancellationToken)
        {
            if (!IsGetPagedForUsersAuthorized())
                return new UnauthorizedResult();

            var Page = 0;
            var PageSize = 10;

            List<BaseClassMU> result = new List<BaseClassMU>();
            var query = Context.Set<OrderStatus>().AsQueryable();

            query = AddGetPagedFiltersForUsers(query);
            int count = await query.CountAsync(cancellationToken);

            query = query.Skip(Page * PageSize).Take(PageSize);

            var list = await query.ToListAsync(cancellationToken);
            result = Mapper.Map(list, result);

            Model.PagedResult<BaseClassMU> pagedResult = new Model.PagedResult<BaseClassMU>();
            pagedResult.ResultList = result;
            pagedResult.Count = count;

            return new OkObjectResult(pagedResult);
        }
        public virtual IQueryable<OrderStatus> AddGetPagedFiltersForUsers(IQueryable<OrderStatus> query)
        {
            query = query.Where(os => os.IsDeleted != true);
            if (_currentUser.IsUser())
            {
                query = query.Where(os => os.ForUsers == true);
            }
            else
            {
                query = query.Where(os => os.ForEmployees == true);
            }

            return query;
        }
        public virtual bool IsUserAuthorized()
        {
            return _currentUser.IsAuthenticated;
        }

    }
}
