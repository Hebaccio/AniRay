using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Services.BaseServices;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.Services.BasicServices
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
        public BasicEntitiesService(AniRayDbContext context, IMapper mapper) : base(context, mapper)
        {
            
        }

        #region Get Filters
        public override IQueryable<TDbEntity> AddFilters(BaseClassUSO search, IQueryable<TDbEntity> query)
        {
            base.AddFilters(search, query);

            query = query.Where(b=> !b.IsDeleted);

            return query;
        }
        public override IQueryable<TDbEntity> AddFiltersEmployees(BaseClassESO search, IQueryable<TDbEntity> query)
        {
            base.AddFiltersEmployees(search, query);

            if (string.IsNullOrEmpty(search.NameFTS))
                query = query.Where(b => b.Name.Contains(search.NameFTS));
            if(search.IsDeleted!=null)
                query = query.Where(b=> b.IsDeleted ==  search.IsDeleted);

            return query;
        }
        public override IQueryable<TDbEntity> AddGetByIdFilters(IQueryable<TDbEntity> query)
        {
            return query.Where(x => !x.IsDeleted);
        }
        public override IQueryable<TDbEntity> AddGetByIdFiltersEmployees(IQueryable<TDbEntity> query)
        {
            return query.Where(x => !x.IsDeleted);
        }
        #endregion

        #region Insert
        public override ServiceResult<bool> BeforeInsertEmployee(BaseClassIR request, TDbEntity entity)
        {
            base.BeforeInsertEmployee(request, entity);

            bool exists = Context.Set<TDbEntity>().Any(x => x.Name == request.Name);
            if (exists)
                return ServiceResult<bool>.Fail($"Name already exists on one {typeof(TDbEntity)} record");

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Update
        public override ServiceResult<bool> BeforeUpdateEmployee(BaseClassEUR request, TDbEntity entity)
        {
            base.BeforeUpdateEmployee(request, entity);

            bool exists = Context.Set<TDbEntity>().Any(x => x.Name == request.Name && x.Id != entity.Id && !x.IsDeleted);

            if (exists)
                return ServiceResult<bool>.Fail($"Name already exists on anohter {typeof(TDbEntity)} record");

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Soft Delete
        public override ServiceResult<string> SoftDelete(int id)
        {
            base.SoftDelete(id);

            var entity = Context.Set<TDbEntity>().Find(id);

            if (entity == null || entity.IsDeleted)
                return ServiceResult<string>.Fail("Entity not found.");

            entity.IsDeleted = true;
            Context.SaveChanges();

            return ServiceResult<string>.Ok("Deleted successfully.");
        }
        #endregion
    }
}
