using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using AniRay.Services.HelperServices.CurrentUserService;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AniRay.Services.BaseServices.BasicEntitiesService
{
    public class BasicEntitiesService<TDbEntity> :
        BaseCRUDService<
            BaseClassUM, BaseClassEM,
            BaseClassUSO, BaseClassESO,
            TDbEntity,
            BaseClassUIR, BaseClassEIR,
            BaseClassUUR, BaseClassEUR>
        where TDbEntity : BaseClass
    {
        private readonly ICurrentUserService _currentUser;

        public BasicEntitiesService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base(context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }

        #region Get By Id - For Users
        //Doesn't Exist
        #endregion

        #region Get By Id - For Employees
        //Doesn't Exist
        #endregion

        #region Get Paged - For Users
        public override IQueryable<TDbEntity> AddGetPagedFiltersForUsers(BaseClassUSO search, IQueryable<TDbEntity> query)
        {
            return query.Where(x => !x.IsDeleted);
        }
        #endregion

        #region Get Paged - For Employees
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
        //Doesn't Exist
        #endregion

        #region Insert - For Employees
        public override async Task<ServiceResult<bool>> BeforeInsertForEmployees(BaseClassEIR request, TDbEntity entity, CancellationToken cancellationToken)
        {
            bool exists = await Context.Set<TDbEntity>().AnyAsync(x => x.Name == request.Name, cancellationToken);
            if (exists)
                return ServiceResult<bool>.Fail($"Name already exists on one {typeof(TDbEntity)} record");

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Update - For Users
        //Doesn't Exist
        #endregion

        #region Update - For Employees
        public override async Task<ServiceResult<bool>> BeforeUpdateForEmployees(BaseClassEUR request, TDbEntity entity, CancellationToken cancellationToken)
        {
            bool exists = await Context.Set<TDbEntity>().AnyAsync(x => x.Name == request.Name && x.Id != entity.Id && !x.IsDeleted, cancellationToken);

            if (exists)
                return ServiceResult<bool>.Fail($"Name already exists on anohter {typeof(TDbEntity)} record");

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region SoftDelete
        public bool IsSoftDeleteAuthorized()
        {
            return _currentUser.IsAuthenticated && _currentUser.IsWorker();
        }
        public override async Task<ActionResult<string>> SoftDelete(int? id, CancellationToken cancellationToken)
        {
            if (!IsSoftDeleteAuthorized())
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
