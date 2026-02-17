using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Interfaces;
using AniRay.Services.Services.BaseServices;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.Services
{
    public class UserCartService : BaseCRUDService<UserCartModel, UserCartSearchObject, UserCart, UserCartInsertRequest, UserCartUpdateRequest>, IUserCartService
    {
        public UserCartService(AniRayDbContext context, IMapper mapper) : base(context, mapper) { }

        public override IQueryable<UserCart> AddFilters(UserCartSearchObject search, IQueryable<UserCart> query)
        {
            query = query.Where(uc => uc.UserId == search.UserId);
            return query;
        }

        public override ServiceResult<bool> BeforeInsert(UserCartInsertRequest request, UserCart entity)
        {
            var userExists = Context.Set<User>().Where(u => u.Id == request.UserId).FirstOrDefault();
            if (userExists == null)
                return ServiceResult<bool>.Fail("User does not exist.");
            
            return ServiceResult<bool>.Ok(true);
        }

        public override ServiceResult<bool> BeforeUpdate(UserCartUpdateRequest request, UserCart entity)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Request cannot be null.");

            entity.CartNotes = request.CartNotes?.Trim();

            var syncResult = SyncCartBluRays(entity, request.BluRay);
            if (!syncResult.Success)
                return syncResult;

            return ServiceResult<bool>.Ok(true);
        }

        private ServiceResult<bool> SyncCartBluRays(UserCart entity, ICollection<BluRayCart>? requestedItems)
        {
            requestedItems ??= new List<BluRayCart>();

            Context.Entry(entity).Collection(x => x.BluRay).Load();

            var groupedItems = requestedItems.GroupBy(x => x.BluRayId)
                .Select(g => new
                {
                    BluRayId = g.Key,
                    Amount = g.Sum(x => x.Amount)
                }).ToList();

            var distinctIds = groupedItems.Select(x => x.BluRayId).ToHashSet();

            var existingCount = Context.Set<BluRay>()
                .Count(b => distinctIds.Contains(b.Id));
            if (existingCount != distinctIds.Count)
                return ServiceResult<bool>.Fail("One or more BluRays do not exist.");

            var currentItems = entity.BluRay.ToDictionary(x => x.BluRayId);

            foreach (var item in groupedItems)
            {
                if (item.Amount <= 0)
                    return ServiceResult<bool>.Fail("Amount must be greater than 0.");

                if (currentItems.TryGetValue(item.BluRayId, out var existing))
                {
                    existing.Amount = item.Amount;
                }
                else
                {
                    entity.BluRay.Add(new BluRayCart
                    {
                        BluRayId = item.BluRayId,
                        Amount = item.Amount
                    });
                }
            }

            var itemsToRemove = entity.BluRay
                .Where(x => !distinctIds.Contains(x.BluRayId))
                .ToList();

            foreach (var item in itemsToRemove)
                entity.BluRay.Remove(item);

            return ServiceResult<bool>.Ok(true);
        }
    }
}
