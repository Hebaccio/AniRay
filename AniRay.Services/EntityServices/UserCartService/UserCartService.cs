using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.HelperRequests;
using AniRay.Model.Requests.UserCartRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using AniRay.Services.HelperServices.CurrentUserService;
using AniRay.Services.HelperServices.OtherHelpers;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AniRay.Services.EntityServices.UserCartService
{
    public class UserCartService :
        BaseCRUDService<UserCartMU, UserCartME, BaseSO, BaseSO, UserCart, UserCartIRU, UserCartIRE, UserCartURU, UserCartURE>, IUserCartService
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
        public override async Task<ServiceResult<bool>> BeforeInsertForUsers(UserCartIRU request, UserCart entity, CancellationToken cancellationToken)
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
        public override async Task<UserCart?> EntityGetTriggerForUpdate(int? id, UserCartURU? request, CancellationToken cancellationToken)
        {
            return await Context.Set<UserCart>().FirstOrDefaultAsync(e => EF.Property<int>(e, "UserId") == _currentUser.UserId, cancellationToken);
        }
        public override async Task<ServiceResult<bool>> BeforeUpdateForUsers(UserCartURU request, UserCart entity, CancellationToken cancellationToken)
        {
            var validationResult = ValidateRequest(request);
            if (!validationResult.Success) return validationResult;
            entity.CartNotes = request.CartNotes;

            var mergedItems = MergeDuplicateItems(request.BluRay);
            request.BluRay = mergedItems;

            var existenceResult = await ValidateExistence(mergedItems, cancellationToken);
            if (!existenceResult.Success) return existenceResult;

            await SyncCartItems(entity, mergedItems, cancellationToken);
            await RecalculateCartPrice(entity, cancellationToken);

            return ServiceResult<bool>.Ok(true);
        }

        private ServiceResult<bool> ValidateRequest(UserCartURU request)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Request cannot be null.");

            ServiceResult<bool> result;
            result = UpsertHelper.ValidateStringLength(request.CartNotes, 1, 500, "Cart Notes", false);
            if(!result.Success) return ServiceResult<bool>.Fail("Cart notes cannot be null.");

            var items = request.BluRay ?? new List<BluRayCartUR>();

            items = MergeDuplicateItems(request.BluRay);
            
            if (AnyBluRaysNull(items))
                return ServiceResult<bool>.Fail("BluRay items cannot be null.");

            if (BluRayCountNotAllowed(items))
                return ServiceResult<bool>.Fail("Maximum amount of BluRay's in cart is 10.");

            if (BluRayAmountNotAllowed(items))
                return ServiceResult<bool>.Fail("Individual BluRay amount must be greater than zero and less than 5.");

            return ServiceResult<bool>.Ok(true);
        }
        private List<BluRayCartUR> MergeDuplicateItems(ICollection<BluRayCartUR>? items)
        {
            if (items == null) return new List<BluRayCartUR>();

            return items.GroupBy(x => x.BluRayId)
                .Select(g => new BluRayCartUR
                {
                    BluRayId = g.Key,
                    Amount = g.Sum(x => x.Amount)
                }).ToList();
        }
        private bool AnyBluRaysNull(ICollection<BluRayCartUR> items)
        {
            if (items.Any(x => x == null))
                return true;

            return false;
        }
        private bool BluRayCountNotAllowed(ICollection<BluRayCartUR> items)
        {
            return items.Count > 10;
        }
        private bool BluRayAmountNotAllowed(ICollection<BluRayCartUR> items)
        {
            foreach (var item in items)
                if (item.Amount < 0 || item.Amount > 5) return true;

            return false;
        }
        private async Task<ServiceResult<bool>> ValidateExistence(List<BluRayCartUR> items, CancellationToken cancellationToken)
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

            RemoveDuplicateCartItems(entity);
            RemoveDeletedCartItems(entity, mergedItems);
            await AddAndUpdateCartItems(entity, mergedItems, cancellationToken);
        }
        private void RemoveDuplicateCartItems(UserCart entity)
        {
            var duplicateGroups = entity.BluRay.GroupBy(x => x.BluRayId).Where(g => g.Count() > 1);

            foreach (var group in duplicateGroups)
            {
                var first = group.First();
                var duplicates = group.Skip(1);

                foreach (var duplicate in duplicates)
                    Context.Set<BluRayCart>().Remove(duplicate);
            }
        }
        private void RemoveDeletedCartItems(UserCart entity, List<BluRayCartUR> mergedItems)
        {
            var existingCartItems = entity.BluRay.GroupBy(x => x.BluRayId).Select(g => g.First()).ToList();
            var requestDict = mergedItems.ToDictionary(x => x.BluRayId);

            foreach (var item in existingCartItems.Where(x => !requestDict.ContainsKey(x.BluRayId)))
                entity.BluRay.Remove(item);
        }
        private async Task AddAndUpdateCartItems(UserCart entity, List<BluRayCartUR> mergedItems, CancellationToken cancellationToken)
        {
            var existingCartItems = entity.BluRay.GroupBy(x => x.BluRayId).Select(g => g.First()).ToList();
            var existingDict = existingCartItems.ToDictionary(x => x.BluRayId);
            var requestDict = mergedItems.ToDictionary(x => x.BluRayId);

            foreach (var item in mergedItems.Where(x => !existingDict.ContainsKey(x.BluRayId)))
            {
                entity.BluRay.Add(new BluRayCart
                {
                    UserCartId = entity.Id,
                    BluRayId = item.BluRayId,
                    Amount = item.Amount
                });
            }

            foreach (var item in existingCartItems.Where(x => requestDict.ContainsKey(x.BluRayId)))
            {
                var newAmount = requestDict[item.BluRayId].Amount;
                if (item.Amount != newAmount)
                    item.Amount = newAmount;
            }

            var bluRayIds = entity.BluRay.Select(x => x.BluRayId).ToList();
            var bluRaysFromDb = await Context.Set<BluRay>().Where(b => bluRayIds.Contains(b.Id)).ToListAsync(cancellationToken);

            var bluRayDict = bluRaysFromDb.ToDictionary(b => b.Id);
            foreach (var cartItem in entity.BluRay)
            {
                cartItem.BluRay = bluRayDict[cartItem.BluRayId];
            }
        }
        private async Task RecalculateCartPrice(UserCart entity, CancellationToken cancellationToken)
        {
            entity.FullCartPrice = entity.BluRay.Sum(x => x.Amount * x.BluRay.Price);
        }
        #endregion

        #region Update - For Employees
        //Doesn't Exist
        #endregion

        #region SoftDelete
        //Doesn't Exist
        #endregion

        #region Other Methods

        #region Is BluRay In Cart
        public async Task<ActionResult<bool>> IsBluRayInCart(int id, CancellationToken cancellationToken)
        {
            if (!IsGetByIdForUsersAuthorized())
                return new UnauthorizedResult();

            IQueryable<BluRayCart> query = Context.Set<BluRayCart>().AsQueryable();
            var entity = await EntityGetTriggerForBluRayInCart(id, query, cancellationToken);
            if (entity == null)
                return new NotFoundObjectResult(false);

            return new OkObjectResult(true);
        }
        public virtual async Task<BluRayCart?> EntityGetTriggerForBluRayInCart(int id, IQueryable<BluRayCart> query, CancellationToken cancellationToken)
        {
            var cart = await Context.Set<UserCart>().Where(uc => uc.UserId == _currentUser.UserId).FirstOrDefaultAsync(cancellationToken);
            if (cart == null)
                return null;

            return await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "UserCartId") == cart.Id && e.BluRay.Id == id, cancellationToken);
        }
        #endregion

        #region Add Individual BluRay to Cart
        public async Task<ActionResult<bool>> AddIndividualBluRayToCart(UserCartIndividualURU request, CancellationToken cancellationToken)
        {
            if (!IsUpdateForUsersAuthorized())
                return new UnauthorizedResult();

            var entity = await EntityGetTriggerForUpdate(request, cancellationToken);
            if (entity == null)
                return new NotFoundObjectResult(new { message = "Entity not found." });

            var validationResult = await BeforeUpdateForUsers(request, entity, cancellationToken);
            if (!validationResult.Success)
                return new BadRequestObjectResult(new { message = validationResult.Message });

            await Context.SaveChangesAsync(cancellationToken);
            await FinalUpdateUserIncludes(entity, request);
            return new OkObjectResult(true);
        }

        public async Task<UserCart?> EntityGetTriggerForUpdate(UserCartIndividualURU? request, CancellationToken cancellationToken)
        {
            return await Context.Set<UserCart>().FirstOrDefaultAsync(e => EF.Property<int>(e, "UserId") == _currentUser.UserId, cancellationToken);
        }
        public Task FinalUpdateUserIncludes(UserCart entity, UserCartIndividualURU? request)
        {
            return Task.CompletedTask;
        }
        public async Task<ServiceResult<bool>> BeforeUpdateForUsers(UserCartIndividualURU request, UserCart entity, CancellationToken cancellationToken)
        {
            await Context.Entry(entity).Collection(c => c.BluRay).LoadAsync(cancellationToken);

            var validationResult = await ValidateRequest(request, entity.BluRay, cancellationToken);
            if (!validationResult.Success) return validationResult;

            await AddBluRayToEntity(request, entity, cancellationToken);
            await RecalculateCartPrice(entity, cancellationToken);

            return ServiceResult<bool>.Ok(true);
        }
        private async Task<ServiceResult<bool>> ValidateRequest(UserCartIndividualURU request, ICollection<BluRayCart> bluRay, CancellationToken cancellationToken)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Request cannot be null.");

            if (request.BluRay == null)
                return ServiceResult<bool>.Fail("BluRay cannot be null.");

            if (bluRay.Count >= 10)
                return ServiceResult<bool>.Fail("Maximum amount of BluRay's in cart is 10.");

            if (request.BluRay.Amount < 0 || request.BluRay.Amount > 5)
                return ServiceResult<bool>.Fail("Individual BluRay amount must be greater than zero and less than 5.");

            var validatingExistence = await ValidateBluRayExistence(request, cancellationToken);
            if (!validatingExistence.Success) return validatingExistence;

            if (BluRayAlreadyInCart(request, bluRay, cancellationToken))
                return ServiceResult<bool>.Fail("Blu Ray is already in cart!");

            return ServiceResult<bool>.Ok(true);
        }
        private async Task<ServiceResult<bool>> ValidateBluRayExistence(UserCartIndividualURU request, CancellationToken cancellationToken)
        {
            var existingBluRays = await Context.Set<BluRay>()
                .Where(b => b.Id == request.BluRay.BluRayId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingBluRays == null)
                return ServiceResult<bool>.Fail("Blu Ray doesn't exist");

            return ServiceResult<bool>.Ok(true);

        }
        private bool BluRayAlreadyInCart(UserCartIndividualURU request, ICollection<BluRayCart> bluRay, CancellationToken cancellationToken)
        {
            foreach(var item in bluRay)
            {
                if(request.BluRay.BluRayId == item.BluRayId)
                    return true;
            }
            return false;
        }
        private async Task AddBluRayToEntity(UserCartIndividualURU request, UserCart entity, CancellationToken cancellationToken)
        {
            await Context.Set<UserCart>().Where(uc => uc.UserId == entity.UserId).Include(uc => uc.BluRay).ThenInclude(uc=> uc.BluRay).FirstOrDefaultAsync(cancellationToken);
            var bluRay = await Context.Set<BluRay>().Where(b => b.Id == request.BluRay.BluRayId).FirstOrDefaultAsync(cancellationToken);

            entity.BluRay.Add(new BluRayCart
            {
                UserCartId = entity.Id,
                BluRayId = request.BluRay.BluRayId,
                BluRay = bluRay!,
                Amount = request.BluRay.Amount
            });
        }
        #endregion

        #region Remove Individual BluRays from Cart
        public async Task<ActionResult<bool>> RemoveIndividualBluRayFromCart(int id, CancellationToken cancellationToken)
        {
            if (!IsUpdateForUsersAuthorized())
                return new UnauthorizedResult();

            var entity = await EntityGetTriggerForUpdate(id, cancellationToken);
            if (entity == null)
                return new NotFoundObjectResult(new { message = "Entity not found." });

            var validationResult = await BeforeRemovingForUsers(id, entity, cancellationToken);
            if (!validationResult.Success)
                return new BadRequestObjectResult(new { message = validationResult.Message });

            await Context.SaveChangesAsync(cancellationToken);
            return new OkObjectResult(true);
        }
        public async Task<UserCart?> EntityGetTriggerForUpdate(int id, CancellationToken cancellationToken)
        {
            return await Context.Set<UserCart>().FirstOrDefaultAsync(e => EF.Property<int>(e, "UserId") == _currentUser.UserId, cancellationToken);
        }
        public async Task<ServiceResult<bool>> BeforeRemovingForUsers(int id, UserCart entity, CancellationToken cancellationToken)
        {
            await Context.Entry(entity).Collection(c => c.BluRay).LoadAsync(cancellationToken);

            var validationResult = await ValidateRequestForRemoving(id, entity.BluRay, cancellationToken);
            if (!validationResult.Success) return validationResult;

            await RemoveBluRayFromEntity(id, entity, cancellationToken);
            await RecalculateCartPrice(entity, cancellationToken);

            return ServiceResult<bool>.Ok(true);
        }
        private async Task<ServiceResult<bool>> ValidateRequestForRemoving(int id, ICollection<BluRayCart> bluRay, CancellationToken cancellationToken)
        {
            var validatingExistence = await ValidateBluRayExistence(id, cancellationToken);
            if (!validatingExistence.Success) return validatingExistence;

            if (!BluRayInCart(id, bluRay, cancellationToken))
                return ServiceResult<bool>.Fail("Blu Ray is not in cart!");

            return ServiceResult<bool>.Ok(true);
        }
        private async Task<ServiceResult<bool>> ValidateBluRayExistence(int id, CancellationToken cancellationToken)
        {
            var existingBluRays = await Context.Set<BluRay>()
                .Where(b => b.Id == id)
                .FirstOrDefaultAsync(cancellationToken);

            if (existingBluRays == null)
                return ServiceResult<bool>.Fail("Blu Ray doesn't exist");

            return ServiceResult<bool>.Ok(true);

        }
        private bool BluRayInCart(int id, ICollection<BluRayCart> bluRay, CancellationToken cancellationToken)
        {
            foreach (var item in bluRay)
            {
                if (id == item.BluRayId)
                    return true;
            }
            return false;
        }
        private async Task RemoveBluRayFromEntity(int id, UserCart entity, CancellationToken cancellationToken)
        {
            await Context.Set<UserCart>().Where(uc => uc.UserId == entity.UserId).Include(uc => uc.BluRay).ThenInclude(uc => uc.BluRay).FirstOrDefaultAsync(cancellationToken);
            var bluRayCart = entity.BluRay.FirstOrDefault(x => x.BluRayId == id);
            if (bluRayCart != null)
                entity.BluRay.Remove(bluRayCart);
        }
        #endregion

        #endregion

    }
}
