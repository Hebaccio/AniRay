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
    public abstract class BaseService<TModel, TSearch, TDbEntity> : IService<TModel, TSearch> where TSearch : BaseSearchObject where TDbEntity : class where TModel : class
    {
        public AniRayDbContext Context { get; set; }
        public IMapper Mapper { get; set; }
        public BaseService(AniRayDbContext context, IMapper mapper)
        {
            Context = context;
            Mapper = mapper;
        }

        #region Gets
        public PagedResult<TModel> GetPaged(TSearch search)
        {
            search.Page = Math.Max(search.Page, 0);
            search.PageSize = Math.Clamp(search.PageSize, 0, 50);

            List<TModel> result = new List<TModel>();
            var query = Context.Set<TDbEntity>().AsQueryable();

            query = AddFilters(search, query);
            int count = query.Count();

            query = query.Skip(search.Page * search.PageSize).Take(search.PageSize);
            
            var list = query.ToList();
            result = Mapper.Map(list, result);

            PagedResult<TModel> pagedResult = new PagedResult<TModel>();
            pagedResult.ResultList = result;
            pagedResult.Count = count;

            return pagedResult;
        }
        public TModel GetById(int id)
        {
            IQueryable<TDbEntity> query = Context.Set<TDbEntity>().AsQueryable();
            query = AddGetByIdFilters(query);

            var entity = query.FirstOrDefault(e => EF.Property<int>(e, "Id") == id);

            if (entity == null)
                return null;

            return Mapper.Map<TModel>(entity);

        }
        #endregion

        #region Filters
        public virtual IQueryable<TDbEntity> AddFilters(TSearch search, IQueryable<TDbEntity> query)
        {
            return query;
        }
        public virtual IQueryable<TDbEntity> AddGetByIdFilters(IQueryable<TDbEntity> query)
        {
            return query;
        }
        #endregion

    }
}
