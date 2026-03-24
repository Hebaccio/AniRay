using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Services.BaseServices.BaseService;
using AniRay.Services.HelperServices.CurrentUserService;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AniRay.Services.BaseServices.BaseCRUDService
{
    public abstract class BaseCRUDService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee, TDbEntity, TInsertUser, TInsertEmployee, TUpdateUser, TUpdateEmployee>
        : BaseService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee, TDbEntity>, 
        ICRUDService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee, TInsertUser, TInsertEmployee, TUpdateUser, TUpdateEmployee>
        where TModelUser : class
        where TModelEmployee : class
        where TSearchUser : BaseSO
        where TSearchEmployee : BaseSO
        where TDbEntity : class
    {
        private readonly ICurrentUserService _currentUser;

        public BaseCRUDService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base(context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }

        #region Insert - For Users
        public virtual async Task<ActionResult<TModelUser>> InsertEntityForUsers(TInsertUser request, CancellationToken cancellationToken)
        {
            if (!IsInsertForUsersAuthorized())
                return new UnauthorizedResult();

            TDbEntity entity = Mapper.Map<TDbEntity>(request);

            var validationResult = await BeforeInsertForUsers(request, entity, cancellationToken);
            if (!validationResult.Success)
                return new BadRequestObjectResult(new { message = validationResult.Message });

            await Context.AddAsync(entity, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);

            await FinalInsertUserIncludes(entity, request);

            var mapped = Mapper.Map<TModelUser>(entity);
            return new OkObjectResult(mapped);
        }

        public virtual Task FinalInsertUserIncludes(TDbEntity entity, TInsertUser request)
        {
            return Task.CompletedTask;
        }

        public virtual bool IsInsertForUsersAuthorized()
        {
            return true;
        }
        public virtual async Task<ServiceResult<bool>> BeforeInsertForUsers(TInsertUser request, TDbEntity entity, CancellationToken cancellationToken)
        {
            return await Task.FromResult(ServiceResult<bool>.Ok(true));
        }
        #endregion

        #region Insert - For Employees
        public virtual async Task<ActionResult<TModelEmployee>> InsertEntityForEmployees(TInsertEmployee request, CancellationToken cancellationToken)
        {
            if (!IsInsertForEmployeesAuthorized())
                return new UnauthorizedResult();

            TDbEntity entity = Mapper.Map<TDbEntity>(request);

            var validationResult = await BeforeInsertForEmployees(request, entity, cancellationToken);
            if (!validationResult.Success)
                return new BadRequestObjectResult(new { message = validationResult.Message });

            await Context.AddAsync(entity, cancellationToken);
            await Context.SaveChangesAsync(cancellationToken);

            await FinalInsertEmployeeIncludes(entity, request);

            var mapped = Mapper.Map<TModelEmployee>(entity);
            return new OkObjectResult(mapped);
        }

        public virtual bool IsInsertForEmployeesAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsWorker();
        }
        public virtual async Task<ServiceResult<bool>> BeforeInsertForEmployees(TInsertEmployee request, TDbEntity entity, CancellationToken cancellationToken)
        {
            return await Task.FromResult(ServiceResult<bool>.Ok(true));
        }
        public virtual Task FinalInsertEmployeeIncludes(TDbEntity entity, TInsertEmployee request)
        {
            return Task.CompletedTask;
        }
        #endregion

        #region Update - For Users
        public virtual async Task<ActionResult<TModelUser>> UpdateEntityForUsers(int? id, TUpdateUser request, CancellationToken cancellationToken)
        {
            if(!IsUpdateForUsersAuthorized())
                return new UnauthorizedResult();

            var entity = await EntityGetTriggerForUpdate(id, request, cancellationToken);
            if (entity == null)
                return new NotFoundObjectResult(new { message = "Entity not found." });

            var validationResult = await BeforeUpdateForUsers(request, entity, cancellationToken);
            if (!validationResult.Success)
                return new BadRequestObjectResult(new { message = validationResult.Message });

            Mapper.Map(request, entity);

            await Context.SaveChangesAsync(cancellationToken);

            await FinalUpdateUserIncludes(entity, request);

            var mapped = Mapper.Map<TModelUser>(entity);
            return new OkObjectResult(mapped);
        }

        public virtual bool IsUpdateForUsersAuthorized()
        {
            return true;
        }
        public virtual async Task<TDbEntity?> EntityGetTriggerForUpdate(int? id, TUpdateUser? request, CancellationToken cancellationToken)
        {
            return await Context.Set<TDbEntity>().FirstOrDefaultAsync(e => EF.Property<int>(e, "Id") == id, cancellationToken);
        }
        public virtual async Task<ServiceResult<bool>> BeforeUpdateForUsers(TUpdateUser request, TDbEntity entity, CancellationToken cancellationToken)
        {
            return await Task.FromResult(ServiceResult<bool>.Ok(true));
        }
        public virtual Task FinalUpdateUserIncludes(TDbEntity entity, TUpdateUser? request)
        {
            return Task.CompletedTask;
        }
        #endregion

        #region Update - For Employees
        public virtual async Task<ActionResult<TModelEmployee>> UpdateEntityForEmployees(int id, TUpdateEmployee request, CancellationToken cancellationToken)
        {
            if(!IsUpdateForEmployeesAuthorized())
                return new UnauthorizedResult();

            var set = Context.Set<TDbEntity>();
            var entity = await set.FindAsync(id, cancellationToken);

            if (entity == null)
                return new NotFoundObjectResult(new { message = "Entity not found." });

            var validationResult = await BeforeUpdateForEmployees(request, entity, cancellationToken);
            if (!validationResult.Success)
                return new BadRequestObjectResult(new { message = validationResult.Message });

            Mapper.Map(request, entity);

            await Context.SaveChangesAsync(cancellationToken);

            await FinalUpdateEmployeeIncludes(entity, request);

            var mapped = Mapper.Map<TModelEmployee>(entity);
            return new OkObjectResult(mapped);
        }

        public virtual bool IsUpdateForEmployeesAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsWorker();
        }
        public virtual async Task<ServiceResult<bool>> BeforeUpdateForEmployees(TUpdateEmployee request, TDbEntity entity, CancellationToken cancellationToken)
        {
            return await Task.FromResult(ServiceResult<bool>.Ok(true));
        }
        public virtual Task FinalUpdateEmployeeIncludes(TDbEntity entity, TUpdateEmployee? request)
        {
            return Task.CompletedTask;
        }
        #endregion

        #region Soft Delete
        public virtual async Task<ActionResult<string>> SoftDelete(int? id, CancellationToken cancellationToken)
        {
            return new ConflictObjectResult(new { message = "This entity cannot be deleted." } );
        }
        #endregion

    }
}
