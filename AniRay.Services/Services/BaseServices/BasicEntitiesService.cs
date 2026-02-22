using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Interfaces;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AniRay.Services.Services.BaseServices
{
    public class BasicEntitiesService<TDbEntity> :
        BaseCRUDService<
            BaseClassUM, BaseClassEM,
            BaseClassUSO, BaseClassESO,
            TDbEntity,
            BaseClassIR, BaseClassIR,
            BaseClassUUR, BaseClassEUR>
        where TDbEntity : BaseClass
    {
        private readonly ICurrentUserService _currentUser;

        public BasicEntitiesService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base(context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }

        #region Get By Id - For Users
        public override IQueryable<TDbEntity> AddGetByIdFiltersForUsers(IQueryable<TDbEntity> query)
        {
            return query.Where(b => !b.IsDeleted);
        }
        #endregion

        #region  Get Paged - For Users
        public override IQueryable<TDbEntity> AddGetPagedFiltersForUsers(BaseClassUSO search, IQueryable<TDbEntity> query)
        {
            return query.Where(x => !x.IsDeleted);
        }
        #endregion

        #region  Get By Id - For Employees
        public override bool IsGetByIdForEmployeesAuthorized()
        {
            if(_currentUser.IsAuthenticated && _currentUser.IsWorker())
                return true;
            return false;
        }
        #endregion

        #region Get Paged - For Employees
        public override bool IsGetPagedForEmployeesAuthorized()
        {
            if (_currentUser.IsAuthenticated && _currentUser.IsWorker())
                return true;
            return false;
        }
        public override IQueryable<TDbEntity> AddGetPagedFiltersForEmployees(BaseClassESO search, IQueryable<TDbEntity> query)
        {
            if (!string.IsNullOrEmpty(search.NameFTS))
                query = query.Where(b => b.Name.Contains(search.NameFTS));
            if (search.IsDeleted != null)
                query = query.Where(b => b.IsDeleted == search.IsDeleted);

            return query;
        }
        #endregion

        #region Insert - For Users
        //It doesn't exist for these types of entities
        #endregion

        #region Update - For Users
        //It doesn't exist for these types of entities
        #endregion

        #region Insert - For Employees
        public override bool IsInsertForEmployeesAuthorized()
        {
            if (_currentUser.IsAuthenticated && _currentUser.IsWorker())
                return true;
            return false;
        }
        public override async Task<ServiceResult<bool>> BeforeInsertForEmployees(BaseClassIR request, TDbEntity entity, CancellationToken cancellationToken)
        {
            bool exists = await Context.Set<TDbEntity>().AnyAsync(x => x.Name == request.Name, cancellationToken);
            if (exists)
                return ServiceResult<bool>.Fail($"Name already exists on one {typeof(TDbEntity)} record");

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Update - For Employees
        public override bool IsUpdateForEmployeesAuthorized()
        {
            if (_currentUser.IsAuthenticated && _currentUser.IsWorker())
                return true;
            return false;
        }
        public override async Task<ServiceResult<bool>> BeforeUpdateForEmployees(BaseClassEUR request, TDbEntity entity, CancellationToken cancellationToken)
        {
            bool exists = await Context.Set<TDbEntity>().AnyAsync(x => x.Name == request.Name && x.Id != entity.Id && !x.IsDeleted, cancellationToken);

            if (exists)
                return ServiceResult<bool>.Fail($"Name already exists on anohter {typeof(TDbEntity)} record");

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Soft Delete
        public override bool IsSoftDeleteAuthorized(int id)
        {
            if (_currentUser.IsAuthenticated && _currentUser.IsWorker())
                return true;
            return false;
        }
        public override async Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            if (!IsSoftDeleteAuthorized(id))
                return new UnauthorizedResult();

            var entity = await Context.Set<TDbEntity>().FindAsync(id, cancellationToken);

            if (entity == null || entity.IsDeleted)
                return new NotFoundObjectResult(new { message = "Entity not found or already deleted." });

            entity.IsDeleted = true;
            await Context.SaveChangesAsync(cancellationToken);

            return new OkObjectResult(new { message = "Deleted successfully." });
        }
        #endregion
    }
}
