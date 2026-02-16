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
        BaseCRUDService<BasicClassModel, BasicClassSearchObject, TDbEntity, BasicClassInsertRequest, BasicClassUpdateRequest>
        where TDbEntity : BaseClass
    {
        public BasicEntitiesService(AniRayDbContext context, IMapper mapper) : base(context, mapper)
        {
            
        }
        #region Get Filters
        public override IQueryable<TDbEntity> AddFilters(BasicClassSearchObject search, IQueryable<TDbEntity> query)
        {
            query = query.Where(x => !x.IsDeleted);
            return query;
        }
        public override IQueryable<TDbEntity> AddGetByIdFilters(IQueryable<TDbEntity> query)
        {
            return query.Where(x => !x.IsDeleted);
        }
        #endregion

        #region Insert
        public override ServiceResult<bool> BeforeInsert(BasicClassInsertRequest request, TDbEntity entity)
        {
            bool exists = Context.Set<TDbEntity>()
                .Any(x => x.Name == request.Name);

            if (exists)
                return ServiceResult<bool>.Fail("Name already exists.");

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Update
        public override ServiceResult<bool> BeforeUpdate(BasicClassUpdateRequest request, TDbEntity entity)
        {
            bool exists = Context.Set<TDbEntity>()
                .Any(x => x.Name == request.Name
                       && x.Id != entity.Id
                       && !x.IsDeleted);

            if (exists)
                return ServiceResult<bool>.Fail("Name already exists.");

            return ServiceResult<bool>.Ok(true);
        }
        #endregion

        #region Soft Delete
        public override ServiceResult<string> SoftDelete(int id)
        {
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
