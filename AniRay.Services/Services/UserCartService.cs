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
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace AniRay.Services.Services
{
    public class UserCartService :
        BaseCRUDService<UserCartUM, UserCartEM, BaseSO, BaseSO, UserCart, UserCartUIR, UserCartEIR, UserCartUUR, UserCartEUR>, IUserCartService
    {
        private readonly ICurrentUserService _currentUser;
        public UserCartService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base (context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }

        #region Get By Id - For Users
        public override bool IsGetByIdForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override IQueryable<UserCart> AddGetByIdFiltersForUsers(IQueryable<UserCart> query)
        {
            query = query.Include(uc => uc.BluRay).ThenInclude(ucb => ucb.BluRay);
            return query;
        }
        public override async Task<UserCart?> EntityGetTrigger(int? id, IQueryable<UserCart> query, CancellationToken cancellationToken)
        {
            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "UserId") == _currentUser.UserId, cancellationToken);
        }
        #endregion

        #region Get By Id - For Employees
        //Doesn't Exist
        #endregion

        #region Get Paged - For Users
        //Doesn't Exist
        #endregion

        #region Get Paged - For Employees
        //Doesn't Exist
        #endregion

        #region Insert - For Users
        public override async Task<ServiceResult<bool>> BeforeInsertForUsers(UserCartUIR request, UserCart entity, CancellationToken cancellationToken)
        {
            entity.UserId = request.UserId;
            entity.FullCartPrice = 0;
            entity.CartNotes = "";

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Insert - For Employees
        //Doesn't Exist
        #endregion

        #region Update - For Users
        public override bool IsUpdateForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override async Task<UserCart?> EntityGetTriggerForUpdate(int? id, UserCartUUR? request, CancellationToken cancellationToken)
        {
            return await Context.Set<UserCart>().FirstOrDefaultAsync(e => EF.Property<int>(e, "UserId") == _currentUser.UserId, cancellationToken);
        }
        public override async Task<ServiceResult<bool>> BeforeUpdateForUsers(UserCartUUR request, UserCart entity, CancellationToken cancellationToken)
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

        private ServiceResult<bool> ValidateRequest(UserCartUUR request)
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

            return ServiceResult<bool>.Ok(true);
        }
        private async Task SyncCartItems(UserCart entity, List<BluRayCartUR> mergedItems, CancellationToken cancellationToken)
        {
            await Context.Entry(entity).Collection(c => c.BluRay).LoadAsync(cancellationToken);

            var duplicateGroups = entity.BluRay.GroupBy(x => x.BluRayId).Where(g => g.Count() > 1).ToList();
            foreach (var group in duplicateGroups)
            {
                var first = group.First();
                var duplicates = group.Skip(1).ToList();

                foreach (var duplicate in duplicates)
                {
                    Context.Set<BluRayCart>().Remove(duplicate);
                }
            }

            var existingCartItems = entity.BluRay.GroupBy(x => x.BluRayId).Select(g => g.First()).ToList();
            var existingDict = existingCartItems.ToDictionary(x => x.BluRayId);
            var requestDict = mergedItems.ToDictionary(x => x.BluRayId);

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

            //await Context.SaveChangesAsync(cancellationToken);
        }
        private async Task RecalculateCartPrice(UserCart entity, CancellationToken cancellationToken)
        {
            await Context.Entry(entity).Collection(c => c.BluRay).Query().Include(x => x.BluRay).LoadAsync(cancellationToken);
            entity.FullCartPrice = entity.BluRay.Sum(x => x.Amount * x.BluRay.Price);
        }
        #endregion

        #region Update - For Employees
        //Doesn't Exist
        #endregion

        #region SoftDelete
        //Doesn't Exist
        #endregion

    }
}
