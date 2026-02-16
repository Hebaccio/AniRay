using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Interfaces;
using AniRay.Services.Services.BaseServices;
using Azure.Core;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Dynamic.Core;
using System.Text;

namespace AniRay.Services.Services
{
    public class OrderService : BaseCRUDService<OrderModel, OrderSearchObject, Order, OrderInsertRequest, OrderUpdateRequest>, IOrderService
    {
        public OrderService(AniRayDbContext context, IMapper mapper) : base (context,mapper) { }

        #region Get Inserts
        public override IQueryable<Order> AddFilters(OrderSearchObject search, IQueryable<Order> query)
        {
            query = base.AddFilters(search, query);

            if (search?.DateTimeGTE != null)
                query = query.Where(o => o.DateTime >= search.DateTimeGTE);

            if (search?.DateTimeLTE != null)
                query = query.Where(o => o.DateTime <= search.DateTimeLTE);

            if (search?.FullPriceGTE != null)
                query = query.Where(o => o.FullPrice >= search.FullPriceGTE);

            if (search?.FullPriceLTE != null)
                query = query.Where(o => o.FullPrice <= search.FullPriceLTE);

            if (search?.OrderStatusId != null)
                query = query.Where(o => o.OrderStatusId == search.OrderStatusId);

            if (!string.IsNullOrEmpty(search?.UserNameFTS))
                query = query.Where(o => o.UserName.Contains(search.UserNameFTS));

            if (!string.IsNullOrEmpty(search?.UserMailFTS))
                query = query.Where(o => o.UserMail.Contains(search.UserMailFTS));

            if (!string.IsNullOrEmpty(search?.UserCountryFTS))
                query = query.Where(o => o.UserCountry.Contains(search.UserCountryFTS));

            if (!string.IsNullOrEmpty(search?.UserCityFTS))
                query = query.Where(o => o.UserCity.Contains(search.UserCityFTS));

            if (!string.IsNullOrEmpty(search?.UserZIPFTS))
                query = query.Where(o => o.UserZIP.Contains(search.UserZIPFTS));

            if (search.OrderBy.HasValue)
            {
                var sort = search.SortType?.ToString() ?? "descending";
                var finalOrderBy = $"{search.OrderBy} {sort}";
                query = query.OrderBy(finalOrderBy);
            }

            query = query.Include(o => o.OrderStatus);
            query = query.Include(o => o.User);
            query = query.Include(o => o.BluRay).ThenInclude(ob => ob.BluRay);

            return query;
        }
        public override IQueryable<Order> AddGetByIdFilters(IQueryable<Order> query)
        {
            query = base.AddGetByIdFilters(query);
            query = query.Include(o => o.OrderStatus);
            query = query.Include(o => o.User);
            query = query.Include(o => o.BluRay).ThenInclude(ob => ob.BluRay);

            return query;
        }
        #endregion

        #region Insert
        public override ServiceResult<bool> BeforeInsert(OrderInsertRequest request, Order entity)
        {
            var exists = CheckIfExists(request.UserId, 1);
            if (!exists.Success)
                return exists;

            entity.OrderStatusId = 1;

            var user = Context.Set<User>().Where(u => u.Id == request.UserId).Select(u => new { u.Username, u.Email }).FirstOrDefault();

            if (user == null)
                return ServiceResult<bool>.Fail("User does not exist.");

            entity.UserName = user.Username;
            entity.UserMail = user.Email;

            if (request.BluRayIds == null || !request.BluRayIds.Any())
                return ServiceResult<bool>.Fail("At least one item must be selected.");

            var groupedItems = request.BluRayIds
            .GroupBy(i => i.BluRayId)
            .Select(g => new
            {
                BluRayId = g.Key,
                Amount = g.Sum(x => x.Amount)
            })
            .ToList();

            var distinctIds = groupedItems
                .Select(i => i.BluRayId)
                .ToList();

            var bluRays = Context.Set<BluRay>()
                .Where(b => distinctIds.Contains(b.Id))
                .ToList();

            if (bluRays.Count != distinctIds.Count)
                return ServiceResult<bool>.Fail("One or more BluRays do not exist.");

            decimal totalPrice = 0;

            foreach (var item in groupedItems)
            {
                if (item.Amount <= 0)
                    return ServiceResult<bool>.Fail("Amount must be greater than 0.");

                var bluRay = bluRays.First(b => b.Id == item.BluRayId);

                totalPrice += bluRay.Price * item.Amount;

                entity.BluRay.Add(new OrderBluRay
                {
                    BluRayId = item.BluRayId,
                    Amount = item.Amount
                });
            }

            entity.FullPrice = totalPrice;
            entity.DateTime = DateTime.UtcNow;

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Update
        public override ServiceResult<bool> BeforeUpdate(OrderUpdateRequest request, Order entity)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Request cannot be null.");

            // Check if the new OrderStatus exists in DB
            var statusExists = Context.Set<OrderStatus>().Any(s => s.Id == request.OrderStatusId);
            if (!statusExists)
                return ServiceResult<bool>.Fail("Selected order status does not exist.");

            // Everything valid
            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Insert/Update Helpers
        private ServiceResult<bool> CheckIfExists(int? userId, int? orderStatusId)
        {
            var userExists = Context.Set<User>().Any(u => u.Id == userId);
            if (!userExists)
                return ServiceResult<bool>.Fail("User does not exist.");

            var statusExists = Context.Set<OrderStatus>().Any(s => s.Id == orderStatusId);
            if (!statusExists)
                return ServiceResult<bool>.Fail("Order status does not exist.");

            return ServiceResult<bool>.Ok(statusExists);
        }
        #endregion

        public override ServiceResult<string> SoftDelete(int id)
        {
            return base.SoftDelete(id);
        }
    }
}
