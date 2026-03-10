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

        #region Get By Id - For Users
        public override bool IsGetByIdForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override IQueryable<Order> AddGetByIdFiltersForUsers(IQueryable<Order> query)
        {
            query = query.Where(o=> o.UserId == _currentUser.UserId).Include(o=> o.BluRay).ThenInclude(o=> o.BluRay);
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
        public override IQueryable<Order> AddGetPagedFiltersForUsers(OrderUSO search, IQueryable<Order> query)
        {
            query = query.Where(o => o.UserId == _currentUser.UserId).OrderByDescending(o=> o.DateTime).Include(o => o.BluRay).ThenInclude(o => o.BluRay);
            query = query.Include(o => o.OrderStatus);
            query = query.Include(o => o.User);
            return query;
        }
        #endregion

        #region Get Paged - For Employees
        public override IQueryable<Order> AddGetPagedFiltersForEmployees(OrderESO search, IQueryable<Order> query)
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
            if(search.OrderBy.HasValue)
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
        public override bool IsInsertForUsersAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsUser();
        }
        public override async Task<ServiceResult<bool>> BeforeInsertForUsers(OrderUIR request, Order entity, CancellationToken cancellationToken)
        {
            var nullCheck = BeforeInsertNullCheck(request);
            if (!nullCheck.Success)
                return nullCheck;

            var validationCheck = BeforeInsertValidation(request);
            if (!validationCheck.Success)
                return validationCheck;

            var bluRayValidation = await ValidateBluRays(request, cancellationToken);
            if (!bluRayValidation.Success)
                return ServiceResult<bool>.Fail(bluRayValidation.Message);

            var bluRaysFromDb = bluRayValidation.Data;

            entity.UserId = _currentUser.UserId.Value;
            entity.User = await Context.Users.FindAsync(entity.UserId, cancellationToken);
            entity.OrderStatusId = (int)CoreData.CoreOrderStatus.InProgress;
            entity.OrderStatus = await Context.OrderStatuses.FindAsync((int)CoreData.CoreOrderStatus.InProgress, cancellationToken);
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

            return ServiceResult<bool>.Ok(true);
        }
        private ServiceResult<bool> BeforeInsertNullCheck(OrderUIR request)
        {
            if (string.IsNullOrEmpty(request.UserPhone))
                return ServiceResult<bool>.Fail("Phone number cannot be null");
            if (string.IsNullOrEmpty(request.UserCountry))
                return ServiceResult<bool>.Fail("Country cannot be null");
            if (string.IsNullOrEmpty(request.UserCity))
                return ServiceResult<bool>.Fail("City cannot be null");
            if (string.IsNullOrEmpty(request.UserZIP))
                return ServiceResult<bool>.Fail("ZIP number cannot be null");
            if (string.IsNullOrEmpty(request.UserAdress))
                return ServiceResult<bool>.Fail("Adress cannot be null");

            return ServiceResult<bool>.Ok(true);
        }
        private ServiceResult<bool> BeforeInsertValidation(OrderUIR request)
        {
            if (request.UserCountry.Length > 50)
                return ServiceResult<bool>.Fail("Country name cannot be longer than 50 characters");
            if (request.UserCity.Length > 50)
                return ServiceResult<bool>.Fail("City name cannot be longer than 50 characters");
            if (request.UserZIP.Length > 20)
                return ServiceResult<bool>.Fail("ZIP number cannot be longer than 20 characters");
            if (request.UserAdress.Length > 100)
                return ServiceResult<bool>.Fail("Adress name cannot be longer than 100 characters");


            return ServiceResult<bool>.Ok(true);
        }
        private async Task<ServiceResult<List<BluRay>>> ValidateBluRays(OrderUIR request, CancellationToken cancellationToken)
        {
            if (request.BluRay == null || !request.BluRay.Any())
                return ServiceResult<List<BluRay>>.Fail("Order must contain at least one BluRay.");

            if (request.BluRay.Any(x => x == null))
                return ServiceResult<List<BluRay>>.Fail("BluRay list contains invalid items.");

            if (request.BluRay.GroupBy(x => x.BluRayId).Any(g => g.Count() > 1))
                return ServiceResult<List<BluRay>>.Fail("Duplicate BluRay entries are not allowed.");

            var bluRayIds = request.BluRay.Select(x => x.BluRayId).ToList();

            var bluRaysFromDb = await Context.Set<BluRay>().Where(b => bluRayIds.Contains(b.Id) && !b.IsDeleted).ToListAsync(cancellationToken);

            if (bluRaysFromDb.Count != bluRayIds.Count)
                return ServiceResult<List<BluRay>>.Fail("One or more BluRays do not exist.");

            foreach (var item in request.BluRay)
            {
                if (item.Amount <= 0 || item.Amount > 5)
                    return ServiceResult<List<BluRay>>.Fail("Amount must be between 1 and 5.");

                var dbBluRay = bluRaysFromDb.First(b => b.Id == item.BluRayId);

                if (item.Amount > dbBluRay.InStock)
                    return ServiceResult<List<BluRay>>.Fail(
                        $"Not enough stock for BluRay '{dbBluRay.Title}'. Available: {dbBluRay.InStock}");
            }

            return ServiceResult<List<BluRay>>.Ok(bluRaysFromDb);
        }
        #endregion

        #region Insert - For Employees
        //Doesn't Exist
        //ACTUALLY
        #endregion

        #region Update - For Users
        //Doesn't Exist
        //ACTUALLY
        #endregion

        #region Update - For Employees
        public override async Task<ServiceResult<bool>> BeforeUpdateForEmployees(OrderEUR request, Order entity, CancellationToken cancellationToken)
        {
            var validationCheck = BeforeUpdateValidation(request);
            if (!validationCheck.Success)
                return validationCheck;

            await Context.Entry(entity).Reference(o => o.OrderStatus).LoadAsync(cancellationToken);
            await Context.Entry(entity).Reference(o => o.User).LoadAsync(cancellationToken);
            await Context.Entry(entity).Collection(o => o.BluRay).Query().Include(ob => ob.BluRay).LoadAsync(cancellationToken);

            if (request.OrderStatusId == (int)CoreOrderStatus.Cancelled || request.OrderStatusId == (int)CoreOrderStatus.Rejected)
            {
                foreach (var ob in entity.BluRay)
                {
                    ob.BluRay.InStock += ob.Amount;
                }
            }

            return ServiceResult<bool>.Ok(true);
        }
        private ServiceResult<bool> BeforeUpdateValidation(OrderEUR request)
        {
            var ValidStatuses = Context.Set<OrderStatus>().ToList();
            foreach (var status in ValidStatuses)
            {
                if(request.OrderStatusId == status.Id)
                    return ServiceResult<bool>.Ok(true);
            }

            return ServiceResult<bool>.Fail("Order Status Id is not valid");
        }
        #endregion

        #region SoftDelete
        //Doesn't Exist
        //ACTUALLY
        #endregion

    }
}
