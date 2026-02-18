using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Services.Interfaces.BaseInterfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AniRay.Services.Services.BaseServices
{
    public abstract class BaseService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee, TDbEntity> :
        IService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee>
        where TModelUser : class
        where TModelEmployee : class
        where TSearchUser : BaseSearchObject
        where TSearchEmployee : BaseSearchObject
        where TDbEntity : class 
    {
        public AniRayDbContext Context { get; set; }
        public IMapper Mapper { get; set; }
        public BaseService(AniRayDbContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }

        public PagedResult<TModelUser> GetPaged(TSearchUser search)
        {
            search.Page = Math.Max(search.Page, 0);
            search.PageSize = Math.Clamp(search.PageSize, 0, 50);

            List<TModelUser> result = new List<TModelUser>();
            var query = Context.Set<TDbEntity>().AsQueryable();

            query = AddFilters(search, query);
            int count = query.Count();

            query = query.Skip(search.Page * search.PageSize).Take(search.PageSize);

            var list = query.ToList();
            result = Mapper.Map(list, result);

            PagedResult<TModelUser> pagedResult = new PagedResult<TModelUser>();
            pagedResult.ResultList = result;
            pagedResult.Count = count;

            return pagedResult;
        }
        public virtual IQueryable<TDbEntity> AddFilters(TSearchUser search, IQueryable<TDbEntity> query)
        {
            return query;
        }

        public TModelUser GetById(int id)
        {
            IQueryable<TDbEntity> query = Context.Set<TDbEntity>().AsQueryable();
            query = AddGetByIdFilters(query);

            var entity = query.FirstOrDefault(e => EF.Property<int>(e, "Id") == id);

            if (entity == null)
                return null;

            return Mapper.Map<TModelUser>(entity);

        }
        public virtual IQueryable<TDbEntity> AddGetByIdFilters(IQueryable<TDbEntity> query)
        {
            return query;
        }

        public PagedResult<TModelEmployee> GetPagedEmployees(TSearchEmployee search)
        {
            search.Page = Math.Max(search.Page, 0);
            search.PageSize = Math.Clamp(search.PageSize, 0, 50);

            List<TModelEmployee> result = new List<TModelEmployee>();
            var query = Context.Set<TDbEntity>().AsQueryable();

            query = AddFiltersEmployees(search, query);
            int count = query.Count();

            query = query.Skip(search.Page * search.PageSize).Take(search.PageSize);

            var list = query.ToList();
            result = Mapper.Map(list, result);

            PagedResult<TModelEmployee> pagedResult = new PagedResult<TModelEmployee>();
            pagedResult.ResultList = result;
            pagedResult.Count = count;

            return pagedResult;
        }
        public virtual IQueryable<TDbEntity> AddFiltersEmployees(TSearchEmployee search, IQueryable<TDbEntity> query)
        {
            return query;
        }

        public TModelEmployee GetByIdEmployees(int id)
        {
            IQueryable<TDbEntity> query = Context.Set<TDbEntity>().AsQueryable();
            query = AddGetByIdFiltersEmployees(query);

            var entity = query.FirstOrDefault(e => EF.Property<int>(e, "Id") == id);

            if (entity == null)
                return null;

            return Mapper.Map<TModelEmployee>(entity);

        }
        public virtual IQueryable<TDbEntity> AddGetByIdFiltersEmployees(IQueryable<TDbEntity> query)
        {
            return query;
        }
    }
}
