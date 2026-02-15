using AniRay.Model;
using AniRay.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using MapsterMapper;
using AniRay.Model.Requests.SearchRequests;

namespace AniRay.Services.Services.BaseServices
{
    public abstract class BaseCRUDService<TModel, TSearch, TDbEntity, TInsert, TUpdate>
        : BaseService<TModel, TSearch, TDbEntity> where TModel : class where TSearch : BaseSearchObject where TDbEntity : class
    {
        public BaseCRUDService(AniRayDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        #region Insert
        public virtual ServiceResult<bool> BeforeInsert(TInsert request, TDbEntity entity)
        {
            return ServiceResult<bool>.Ok(true);
        }
        public virtual ServiceResult<TModel> Insert(TInsert request)
        {
            TDbEntity entity = Mapper.Map<TDbEntity>(request);

            var validationResult = BeforeInsert(request, entity);
            if (!validationResult.Success)
                return ServiceResult<TModel>.Fail(validationResult.Message!);

            Context.Add(entity);
            Context.SaveChanges();

            return ServiceResult<TModel>.Ok(Mapper.Map<TModel>(entity));
        }
        #endregion

        #region Update
        public virtual ServiceResult<bool> BeforeUpdate(TUpdate request, TDbEntity entity)
        {
            return ServiceResult<bool>.Ok(true);
        }
        public virtual ServiceResult<TModel> Update(int id, TUpdate request)
        {
            var set = Context.Set<TDbEntity>();
            var entity = set.Find(id);

            if (entity == null)
                return ServiceResult<TModel>.Fail("Entity not found.");

            Mapper.Map(request, entity);

            var validationResult = BeforeUpdate(request, entity);
            if (!validationResult.Success)
                return ServiceResult<TModel>.Fail(validationResult.Message!);

            Context.SaveChanges();

            return ServiceResult<TModel>.Ok(Mapper.Map<TModel>(entity));
        }
        #endregion

        #region Soft Delete
        public virtual ServiceResult<string> SoftDelete(int id)
        {
            return ServiceResult<string>.Ok($"This entity cannot be deleted");
        }
        #endregion
    }
}
