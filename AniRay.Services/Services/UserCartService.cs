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
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.Services
{
    public class UserCartService :
        BaseCRUDService<UserCartUM, UserCartUM, BaseSO, BaseSO, UserCart, UserCartIR, UserCartIR, UserCartUR, UserCartUR>, IUserCartService
    {
        private readonly ICurrentUserService _currentUser;
        public UserCartService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base (context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }

        #region Insert - For User
        public override async Task<ServiceResult<bool>> BeforeInsertForUsers(UserCartIR request, UserCart entity, CancellationToken cancellationToken)
        {
            entity.UserId = request.UserId;
            entity.FullCartPrice = 0;
            entity.CartNotes = "";

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Get By Id - For User
        public override bool IsGetByIdForUsersAuthorized(int? id)
        {
            return _currentUser.IsAuthenticated && (_currentUser.IsUser() && _currentUser.IsSelf(id.Value));
        }
        public override async Task<ActionResult<UserCartUM>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            if (!IsGetByIdForUsersAuthorized(id))
                return new UnauthorizedResult();
            IQueryable<UserCart> query = Context.Set<UserCart>().AsQueryable();
            query = AddGetByIdFiltersForUsers(query);

            var entity = await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "UserId") == id, cancellationToken);

            if (entity == null)
                return new NotFoundObjectResult(new { message = $"UserCart for user with {id} not found." });

            var mapped = Mapper.Map<UserCartUM>(entity);

            return new OkObjectResult(mapped);
        }
        public override IQueryable<UserCart> AddGetByIdFiltersForUsers(IQueryable<UserCart> query)
        {
            query = query.Include(uc => uc.BluRay).ThenInclude(ucb=> ucb.BluRay);
            return query;
        }
        #endregion

        #region Update - For Users
        public override bool IsUpdateForUsersAuthorized(int? id)
        {
            return _currentUser.IsAuthenticated && (_currentUser.IsUser() && _currentUser.IsSelf(id.Value));
        }
        public override async Task<ActionResult<UserCartUM>> UpdateEntityForUsers(int id, UserCartUR request, CancellationToken cancellationToken)
        {
            var set = Context.Set<UserCart>();
            var entity = await set.FindAsync(id, cancellationToken);
            if (entity == null)
                return new NotFoundObjectResult(new { message = "Entity not found." });
            
            if (!IsUpdateForUsersAuthorized(entity.UserId))
                return new UnauthorizedResult();

            var validationResult = await BeforeUpdateForUsers(request, entity, cancellationToken);
            if (!validationResult.Success)
                return new BadRequestObjectResult(new { message = validationResult.Message });

            Mapper.Map(request, entity);
            await Context.SaveChangesAsync(cancellationToken);

            var mapped = Mapper.Map<UserCartUM>(entity);
            return new OkObjectResult(mapped);
        }

        public override async Task<ServiceResult<bool>> BeforeUpdateForUsers(UserCartUR request, UserCart entity, CancellationToken cancellationToken)
        {
            var validationResult = ValidateRequest(request);
            if (!validationResult.Success) return validationResult;
            entity.CartNotes = request.CartNotes;

            var mergedItems = MergeDuplicateItems(request.BluRay);
            request.BluRay = mergedItems;

            var existenceResult = await ValidateExistenceAndStock(mergedItems, cancellationToken);
            if (!existenceResult.Success) return existenceResult;

            await SyncCartItems(entity, mergedItems, cancellationToken);
            await RecalculateCartPrice(entity, cancellationToken);

            return ServiceResult<bool>.Ok(true);
        }

        private ServiceResult<bool> ValidateRequest(UserCartUR request)
        {
            if (request.CartNotes == null)
                return ServiceResult<bool>.Fail("Cart notes cannot be null.");

            var items = request.BluRay ?? new List<BluRayCartUR>();

            if (items.Any(x => x == null))
                return ServiceResult<bool>.Fail("BluRay items cannot be null.");

            if (items.Any(x => x.Amount <= 0 || x.Amount > 5))
                return ServiceResult<bool>.Fail("Individual BluRay amount must be greater than zero and less than 5.");

            var merged = MergeDuplicateItems(items);
            if (merged.Count > 10)
                return ServiceResult<bool>.Fail("Maximum amount of BluRay's in cart is 10.");

            return ServiceResult<bool>.Ok(true);
        }
        private List<BluRayCartUR> MergeDuplicateItems(ICollection<BluRayCartUR>? items)
        {
            if (items == null) return new List<BluRayCartUR>();

            return items
                .GroupBy(x => x.BluRayId)
                .Select(g => new BluRayCartUR
                {
                    BluRayId = g.Key,
                    Amount = g.Sum(x => x.Amount)
                }).ToList();
        }
        private async Task<ServiceResult<bool>> ValidateExistenceAndStock(List<BluRayCartUR> items, CancellationToken cancellationToken)
        {
            var requestedIds = items.Select(x => x.BluRayId).ToList();

            var existingBluRays = await Context.Set<BluRay>()
                .Where(b => requestedIds.Contains(b.Id))
                .ToListAsync(cancellationToken);

            if (existingBluRays.Count != requestedIds.Count)
                return ServiceResult<bool>.Fail("Some BluRay Ids cannot be found in the database.");

            /*foreach (var item in items)
            {
                var bluRay = existingBluRays.First(b => b.Id == item.BluRayId);
                if (item.Amount > bluRay.InStock)
                    return ServiceResult<bool>.Fail(
                        $"Requested amount ({item.Amount}) for BluRay '{bluRay.Title}' exceeds available stock ({bluRay.InStock})."
                    );
            }*/

            return ServiceResult<bool>.Ok(true);
        }
        private async Task SyncCartItems(UserCart entity, List<BluRayCartUR> mergedItems, CancellationToken cancellationToken)
        {
            await Context.Entry(entity).Collection(c => c.BluRay).LoadAsync(cancellationToken);

            var existingCartItems = entity.BluRay.ToList();
            var existingDict = existingCartItems.ToDictionary(x => x.BluRayId, x => x);
            var requestDict = mergedItems.ToDictionary(x => x.BluRayId, x => x);

            foreach (var item in existingCartItems.Where(x => !requestDict.ContainsKey(x.BluRayId)).ToList())
                Context.Set<BluRayCart>().Remove(item);

            foreach (var item in mergedItems.Where(x => !existingDict.ContainsKey(x.BluRayId)))
                entity.BluRay.Add(new BluRayCart
                {
                    UserCartId = entity.Id,
                    BluRayId = item.BluRayId,
                    Amount = item.Amount
                });

            foreach (var item in existingCartItems.Where(x => requestDict.ContainsKey(x.BluRayId)))
            {
                var newAmount = requestDict[item.BluRayId].Amount;
                if (item.Amount != newAmount)
                    item.Amount = newAmount;
            }

            await Context.SaveChangesAsync(cancellationToken);
        }
        private async Task RecalculateCartPrice(UserCart entity, CancellationToken cancellationToken)
        {
            await Context.Entry(entity)
                .Collection(c => c.BluRay)
                .Query()
                .Include(x => x.BluRay)
                .LoadAsync(cancellationToken);

            entity.FullCartPrice = entity.BluRay.Sum(x => x.Amount * x.BluRay.Price);
        }
        #endregion


    }
}
