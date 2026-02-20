using AniRay.Model;
using AniRay.Model.Data;
using System;
using System.Collections.Generic;
using System.Text;
using MapsterMapper;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Services.Interfaces.BaseInterfaces;

namespace AniRay.Services.Services.BaseServices
{
    public abstract class BaseCRUDService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee, TDbEntity, TInsertUser, TInserEmployee, TUpdateUser, TUpdateEmployee>
        : BaseService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee, TDbEntity>, 
        ICRUDService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee, TInsertUser, TInserEmployee, TUpdateUser, TUpdateEmployee>
        where TModelUser : class
        where TModelEmployee : class
        where TSearchUser : BaseSO
        where TSearchEmployee : BaseSO
        where TDbEntity : class
    {
        public BaseCRUDService(AniRayDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public virtual ServiceResult<TModelUser> InsertEntityForUsers(TInsertUser request)
        {
            TDbEntity entity = Mapper.Map<TDbEntity>(request);

            var validationResult = BeforeInsert(request, entity);
            if (!validationResult.Success)
                return ServiceResult<TModelUser>.Fail(validationResult.Message!);

            Context.Add(entity);
            Context.SaveChanges();

            return ServiceResult<TModelUser>.Ok(Mapper.Map<TModelUser>(entity));
        }
        public virtual ServiceResult<bool> BeforeInsert(TInsertUser request, TDbEntity entity)
        {
            return ServiceResult<bool>.Ok(true);
        }

        public virtual ServiceResult<TModelEmployee> InsertEntityForEmployees(TInserEmployee request)
        {
            TDbEntity entity = Mapper.Map<TDbEntity>(request);

            var validationResult = BeforeInsertEmployee(request, entity);
            if (!validationResult.Success)
                return ServiceResult<TModelEmployee>.Fail(validationResult.Message!);

            Context.Add(entity);
            Context.SaveChanges();

            return ServiceResult<TModelEmployee>.Ok(Mapper.Map<TModelEmployee>(entity));
        }
        public virtual ServiceResult<bool> BeforeInsertEmployee(TInserEmployee request, TDbEntity entity)
        {
            return ServiceResult<bool>.Ok(true);
        }
        
        public virtual ServiceResult<TModelUser> UpdateEntityForUsers(int id, TUpdateUser request)
        {
            var set = Context.Set<TDbEntity>();
            var entity = set.Find(id);

            if (entity == null)
                return ServiceResult<TModelUser>.Fail("Entity not found.");

            Mapper.Map(request, entity);

            var validationResult = BeforeUpdate(request, entity);
            if (!validationResult.Success)
                return ServiceResult<TModelUser>.Fail(validationResult.Message!);

            Context.SaveChanges();

            return ServiceResult<TModelUser>.Ok(Mapper.Map<TModelUser>(entity));
        }
        public virtual ServiceResult<bool> BeforeUpdate(TUpdateUser request, TDbEntity entity)
        {
            return ServiceResult<bool>.Ok(true);
        }

        public virtual ServiceResult<TModelEmployee> UpdateEntityForEmployees(int id, TUpdateEmployee request)
        {
            var set = Context.Set<TDbEntity>();
            var entity = set.Find(id);

            if (entity == null)
                return ServiceResult<TModelEmployee>.Fail("Entity not found.");

            Mapper.Map(request, entity);

            var validationResult = BeforeUpdateEmployee(request, entity);
            if (!validationResult.Success)
                return ServiceResult<TModelEmployee>.Fail(validationResult.Message!);

            Context.SaveChanges();

            return ServiceResult<TModelEmployee>.Ok(Mapper.Map<TModelEmployee>(entity));
        }
        public virtual ServiceResult<bool> BeforeUpdateEmployee(TUpdateEmployee request, TDbEntity entity)
        {
            return ServiceResult<bool>.Ok(true);
        }

        public virtual ServiceResult<string> SoftDelete(int id)
        {
            return ServiceResult<string>.Ok($"This entity cannot be deleted");
        }
    }
}
