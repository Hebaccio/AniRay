using AniRay.Model;
using AniRay.Model.Requests.SearchRequests;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.Interfaces.BaseInterfaces
{
    public interface IService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee>
        where TSearchUser : BaseSearchObject
        where TSearchEmployee : BaseSearchObject
    {
        public PagedResult<TModelEmployee> GetPagedEmployees(TSearchEmployee search);
        public TModelEmployee GetByIdEmployees(int id);
        public PagedResult<TModelUser> GetPaged(TSearchUser search);
        public TModelUser GetById(int id);
    }
}
