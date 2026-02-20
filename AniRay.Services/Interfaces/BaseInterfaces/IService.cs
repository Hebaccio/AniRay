using AniRay.Model;
using AniRay.Model.Requests.SearchRequests;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.Interfaces.BaseInterfaces
{
    public interface IService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee>
        where TSearchUser : BaseSO
        where TSearchEmployee : BaseSO
    {
        public PagedResult<TModelEmployee> GetPagedEntitiesForEmployees(TSearchEmployee search);
        public TModelEmployee EntityGetByIdForEmployees(int id);
        public PagedResult<TModelUser> GetPagedEntityForUsers(TSearchUser search);
        public TModelUser EntityGetByIdForUsers(int id);
    }
}
