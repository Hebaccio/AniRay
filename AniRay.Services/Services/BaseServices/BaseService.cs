using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Services.Interfaces;
using AniRay.Services.Interfaces.BaseInterfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AniRay.Services.Services.BaseServices
{
    public abstract class BaseService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee, TDbEntity> :
        IService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee>
        where TModelUser : class
        where TModelEmployee : class
        where TSearchUser : BaseSO
        where TSearchEmployee : BaseSO
        where TDbEntity : class 
    {
        public AniRayDbContext Context { get; set; }
        public IMapper Mapper { get; set; }
        private readonly ICurrentUserService _currentUserService;

        public BaseService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUserService)
        {
            Context = context;
            Mapper = mapper;
            _currentUserService = currentUserService;
        }

        #region Get By Id - For Users
        public virtual async Task<ActionResult<TModelUser>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            if (!IsGetByIdForUsersAuthorized(id))
                return new UnauthorizedResult();
            IQueryable<TDbEntity> query = Context.Set<TDbEntity>().AsQueryable();
            query = AddGetByIdFiltersForUsers(query);

            var entity = await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, cancellationToken);

            if (entity == null)
                return new NotFoundObjectResult(new { message = $"Entity with ID {id} not found." });

            var mapped = Mapper.Map<TModelUser>(entity);

            return new OkObjectResult(mapped);

        }

        public virtual bool IsGetByIdForUsersAuthorized(int? id)
        {
            return true;
        }

        public virtual IQueryable<TDbEntity> AddGetByIdFiltersForUsers(IQueryable<TDbEntity> query)
        {
            return query;
        }
        #endregion

        #region Get Paged - For Users
        public virtual async Task<ActionResult<PagedResult<TModelUser>>> GetPagedEntityForUsers(TSearchUser search, CancellationToken cancellationToken)
        {
            if (!IsGetPagedForUsersAuthorized())
                return new UnauthorizedResult();

            search.Page = Math.Max(search.Page, 0);
            search.PageSize = Math.Clamp(search.PageSize, 0, 50);

            List<TModelUser> result = new List<TModelUser>();
            var query = Context.Set<TDbEntity>().AsQueryable();

            query = AddGetPagedFiltersForUsers(search, query);
            int count = await query.CountAsync(cancellationToken);

            query = query.Skip(search.Page * search.PageSize).Take(search.PageSize);

            var list = await query.ToListAsync(cancellationToken);
            result = Mapper.Map(list, result);

            PagedResult<TModelUser> pagedResult = new PagedResult<TModelUser>();
            pagedResult.ResultList = result;
            pagedResult.Count = count;

            return new OkObjectResult(pagedResult);
        }

        public virtual bool IsGetPagedForUsersAuthorized()
        {
            return true;
        }

        public virtual IQueryable<TDbEntity> AddGetPagedFiltersForUsers(TSearchUser search, IQueryable<TDbEntity> query)
        {
            return query;
        }
        #endregion

        #region Get By Id - For Employees
        public virtual async Task<ActionResult<TModelEmployee>> EntityGetByIdForEmployees(int id, CancellationToken cancellationToken)
        {
            if (!IsGetByIdForEmployeesAuthorized())
                return new UnauthorizedResult();

            IQueryable<TDbEntity> query = Context.Set<TDbEntity>().AsQueryable();
            query = AddGetByIdFiltersForEmployees(query);

            var entity = await query.FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, cancellationToken);

            if (entity == null)
                return new NotFoundObjectResult(new { message = $"Entity with ID {id} not found." });

            var mapped = Mapper.Map<TModelEmployee>(entity);

            return new OkObjectResult(mapped);
        }

        public virtual bool IsGetByIdForEmployeesAuthorized()
        {
            return true;
        }

        public virtual IQueryable<TDbEntity> AddGetByIdFiltersForEmployees(IQueryable<TDbEntity> query)
        {
            return query;
        }
        #endregion

        #region Get Paged - For Employees
        public virtual async Task<ActionResult<PagedResult<TModelEmployee>>> GetPagedEntityForEmployees(TSearchEmployee search, CancellationToken cancellationToken)
        {
            if (!IsGetPagedForEmployeesAuthorized())
                return new UnauthorizedResult();

            search.Page = Math.Max(search.Page, 0);
            search.PageSize = Math.Clamp(search.PageSize, 0, 50);

            List<TModelEmployee> result = new List<TModelEmployee>();
            var query = Context.Set<TDbEntity>().AsQueryable();

            query = AddGetPagedFiltersForEmployees(search, query);
            int count = await query.CountAsync(cancellationToken);

            query = query.Skip(search.Page * search.PageSize).Take(search.PageSize);

            var list = await query.ToListAsync(cancellationToken);
            result = Mapper.Map(list, result);

            PagedResult<TModelEmployee> pagedResult = new PagedResult<TModelEmployee>();
            pagedResult.ResultList = result;
            pagedResult.Count = count;

            return new OkObjectResult(pagedResult);
        }

        public virtual bool IsGetPagedForEmployeesAuthorized()
        {
            return true;
        }

        public virtual IQueryable<TDbEntity> AddGetPagedFiltersForEmployees(TSearchEmployee search, IQueryable<TDbEntity> query)
        {
            return query;
        }
        #endregion

    }
}
