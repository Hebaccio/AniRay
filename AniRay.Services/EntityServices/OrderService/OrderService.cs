using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.OrderRequests;
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
        BaseCRUDService<OrderMU, OrderME, OrderSOU, OrderSOE, Order, OrderIRU, OrderIRE, OrderURU, OrderURE>, IOrderService
    {
        private readonly ICurrentUserService _currentUser;
        public OrderService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser)
            :base(context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }

        #region Get By Id - For Users
        public override bool IsGetByIdForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override IQueryable<Order> AddGetByIdFiltersForUsers(IQueryable<Order> query)
        {
            query = query.Where(o => o.UserId == _currentUser.UserId).Include(o => o.BluRay).ThenInclude(o => o.BluRay);
            query = query.Include(o => o.OrderStatus);
            return query;
        }
        public override async Task<Order?> EntityGetTrigger(int? id, IQueryable<Order> query, CancellationToken cancellationToken)
        {
            return await query.Where(e => EF.Property<int>(e, "Id") == id).Include(e => e.User).FirstOrDefaultAsync(cancellationToken);
        }
        #endregion

        #region Get By Id - For Employees
        public override IQueryable<Order> AddGetByIdFiltersForEmployees(IQueryable<Order> query)
        {
            query = query.Include(o => o.BluRay).ThenInclude(o => o.BluRay);
            query = query.Include(o => o.OrderStatus);
            return query;
        }
        public override async Task<Order?> EntityGetTriggerForEmployee(int? id, IQueryable<Order> query, CancellationToken cancellationToken)
        {
            return await query.Where(e => EF.Property<int>(e, "Id") == id).Include(e => e.User).FirstOrDefaultAsync(cancellationToken);

        }
        #endregion

        #region Get Paged - For Users
        public override bool IsGetPagedForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override IQueryable<Order> AddGetPagedFiltersForUsers(OrderSOU search, IQueryable<Order> query)
        {
            query = query.Where(o => o.UserId == _currentUser.UserId).OrderByDescending(o => o.DateTime).Include(o => o.BluRay).ThenInclude(o => o.BluRay);
            query = query.Include(o => o.OrderStatus);
            query = query.Include(o => o.User);
            return query;
        }
        #endregion

        #region Get Paged - For Employees
        public override IQueryable<Order> AddGetPagedFiltersForEmployees(OrderSOE search, IQueryable<Order> query)
        {
            if (!string.IsNullOrWhiteSpace(search.UserNameFTS))
                query = query.Where(o => o.User.Name.Contains(search.UserNameFTS));

            if (!string.IsNullOrWhiteSpace(search.UserMailFTS))
                query = query.Where(o => o.User.Email.Contains(search.UserMailFTS));

            if (search.DateTimeGTE != null)
                query = query.Where(o => o.DateTime >= search.DateTimeGTE);
            if (search.DateTimeLTE != null)
                query = query.Where(o => o.DateTime <= search.DateTimeLTE);
            if (search.FullPriceGTE != null)
                query = query.Where(o => o.FullPrice >= search.FullPriceGTE);
            if (search.FullPriceLTE != null)
                query = query.Where(o => o.FullPrice <= search.FullPriceLTE);
            if (search.OrderStatusId != null)
                query = query.Where(o => o.OrderStatusId == search.OrderStatusId);
            if (search.UserId != null)
                query = query.Where(o => o.UserId == search.UserId);
            if (!string.IsNullOrWhiteSpace(search.UserCountryFTS))
                query = query.Where(o => o.UserCountry.Contains(search.UserCountryFTS));
            if (!string.IsNullOrWhiteSpace(search.UserCityFTS))
                query = query.Where(o => o.UserCity.Contains(search.UserCityFTS));
            if (!string.IsNullOrWhiteSpace(search.UserZIPFTS))
                query = query.Where(o => o.UserZIP.Contains(search.UserZIPFTS));
            if (search.OrderBy.HasValue)
            {
                var sort = search.SortType?.ToString() ?? "descending";
                var orderBy = search.OrderBy?.ToString() ?? "DateTime";
                var finalOrderBy = $"{orderBy} {sort}";
                query = query.OrderBy(finalOrderBy);
            }

            query = query.Include(o => o.BluRay).ThenInclude(o => o.BluRay);
            query = query.Include(o => o.OrderStatus);
            query = query.Include(o => o.User);

            return query;
        }
        #endregion

        #region Insert - For Users
        public override bool IsDeleteForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override async Task<ServiceResult<bool>> BeforeInsertForUsers(OrderIRU request, Order entity, CancellationToken cancellationToken)
        {
            if (request == null)
                return ServiceResult<bool>.Fail("Request field cannot be null!");

            var validationCheck = await BeforeInsertChecks(request, entity, cancellationToken);
            if (!validationCheck.Success)
                return validationCheck;

            return ServiceResult<bool>.Ok(true);
        }

        private async Task<ServiceResult<bool>> BeforeInsertChecks(OrderIRU request, Order entity, CancellationToken cancellationToken)
        {
            ServiceResult<bool> result;

            //Country
            result = UpsertHelper.ValidateStringLength(request.UserCountry, 1, 50, "Country", false);
            if (!result.Success) return result;

            //City
            result = UpsertHelper.ValidateStringLength(request.UserCity, 1, 50, "City", false);
            if (!result.Success) return result;

            //ZIP
            result = UpsertHelper.ValidateStringLength(request.UserZIP, 1, 20, "ZIP Code", false);
            if (!result.Success) return result;

            //Adress
            result = UpsertHelper.ValidateStringLength(request.UserAdress, 1, 100, "Adress", false);
            if (!result.Success) return result;

            //User Notes
            result = UpsertHelper.ValidateStringLength(request.UserNotes, 1, 300, "Notes", true);
            if (!result.Success) return result;

            //Phone
            result = UpsertHelper.ValidatePhoneRegex(request.UserPhone, 1, 30, "Phone", false);
            if (!result.Success) return result;

            //BluRays
            var bluRayValidation = await ValidateBluRays(request, cancellationToken);
            if (!bluRayValidation.Success)
                return ServiceResult<bool>.Fail(bluRayValidation.Message);

            await PopulateOrderEntity(request, entity, bluRayValidation.Data!, cancellationToken);

            return ServiceResult<bool>.Ok(true);
        }
        private async Task<ServiceResult<List<BluRay>>> ValidateBluRays(OrderIRU request, CancellationToken cancellationToken)
        {
            var errors = new List<string>();

            if (request.BluRay == null || !request.BluRay.Any())
                return ServiceResult<List<BluRay>>.Fail("Order must contain at least one BluRay.");

            if (request.BluRay.Any(x => x == null))
                return ServiceResult<List<BluRay>>.Fail("BluRay list contains invalid items.");

            if(request.BluRay.Count > 10)
                return ServiceResult<List<BluRay>>.Fail("An Order can only contain 10 Blu Rays within it.");

            if (request.BluRay.GroupBy(x => x.BluRayId).Any(g => g.Count() > 1))
                return ServiceResult<List<BluRay>>.Fail("Duplicate BluRay entries are not allowed.");

            var bluRayIds = request.BluRay.Select(x => x.BluRayId).ToList();
            var bluRaysFromDb = await Context.Set<BluRay>().Where(b => bluRayIds.Contains(b.Id) && !b.IsDeleted).ToListAsync(cancellationToken);

            if (bluRaysFromDb.Count != bluRayIds.Count)
                return ServiceResult<List<BluRay>>.Fail("One or more BluRays do not exist.");

            foreach (var item in request.BluRay)
            {
                if (item.Amount <= 0 || item.Amount > 5)
                {
                    return ServiceResult<List<BluRay>>.Fail(
                        $"Amount for BluRayId {item.BluRayId} must be between 1 and 5.");
                }

                var dbBluRay = bluRaysFromDb.FirstOrDefault(b => b.Id == item.BluRayId);

                if (dbBluRay == null)
                    continue;

                if (item.Amount > dbBluRay.InStock)
                {
                    if (!request.BluRayAmountChange)
                    {
                        errors.Add(
                            $"Not enough BluRay's in stock for '{dbBluRay.Title}'. Available: {dbBluRay.InStock}");
                    }
                    else
                    {
                        item.Amount = dbBluRay.InStock;
                    }
                }
            }

            if (errors.Any())
                return ServiceResult<List<BluRay>>.Fail("One or more BluRays in your order has less stock than your order requires." +
                    " Would you like us to adjust your order so that every Blu Ray amount in it has the whole remaining stock?");

            return ServiceResult<List<BluRay>>.Ok(bluRaysFromDb);
        }
        private async Task PopulateOrderEntity(OrderIRU request, Order entity, List<BluRay> bluRaysFromDb, CancellationToken cancellationToken)
        {
            entity.UserId = _currentUser.UserId!.Value;
            entity.User = await Context.Users.FindAsync(entity.UserId, cancellationToken);

            entity.OrderStatusId = (int)CoreData.CoreOrderStatus.InProgress;
            entity.OrderStatus = await Context.OrderStatuses.FindAsync(entity.OrderStatusId, cancellationToken);

            entity.DateTime = DateTime.UtcNow;
            decimal totalPrice = 0;

            foreach (var item in request.BluRay)
            {
                var dbBluRay = bluRaysFromDb.First(b => b.Id == item.BluRayId);
                dbBluRay.InStock -= item.Amount;

                entity.BluRay.Add(new OrderBluRay
                {
                    BluRayId = item.BluRayId,
                    Amount = item.Amount
                });

                totalPrice += dbBluRay.Price * item.Amount;
            }

            entity.FullPrice = totalPrice;
        }
        #endregion

        #region Insert - For Employees
        //Doesn't Exist
        #endregion

        #region Update - For Users
        //Doesn't Exist
        #endregion

        #region Update - For Employees
        public override async Task<ServiceResult<bool>> BeforeUpdateForEmployees(OrderURE request, Order entity, CancellationToken cancellationToken)
        {
            var validationCheck = BeforeUpdateValidation(request);
            if (!validationCheck.Success)
                return validationCheck;

            await Context.Entry(entity).Reference(o => o.OrderStatus).LoadAsync(cancellationToken);
            await Context.Entry(entity).Reference(o => o.User).LoadAsync(cancellationToken);
            await Context.Entry(entity).Collection(o => o.BluRay).Query().Include(ob => ob.BluRay).LoadAsync(cancellationToken);

            if (CheckOrderStatusIds(entity.OrderStatusId, request.OrderStatusId))
            {
                foreach (var ob in entity.BluRay)
                {
                    ob.BluRay.InStock += ob.Amount;
                }
            }

            return ServiceResult<bool>.Ok(true);
        }
        private bool CheckOrderStatusIds(int entityOrderStatusId, int requestOrderStatusId)
        {
            return entityOrderStatusId == (int)CoreOrderStatus.InProgress &&
                (requestOrderStatusId == (int)CoreOrderStatus.Cancelled ||
                 requestOrderStatusId == (int)CoreOrderStatus.Rejected);
        }
        private ServiceResult<bool> BeforeUpdateValidation(OrderURE request)
        {
            var exists = Context.Set<OrderStatus>().Any(x => x.Id == request.OrderStatusId);
            if (!exists)
                return ServiceResult<bool>.Fail("Order Status Id is not valid");

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region SoftDelete
        //Doesn't Exist
        #endregion

    }
}
