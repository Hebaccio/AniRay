using AniRay.Model;
using AniRay.Model.Requests.SearchRequests;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.Interfaces.BaseInterfaces
{
    public interface ICRUDService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee, TInsertUser, TInsertEmployee, TUpdateUser, TUpdateEmployee> :
        IService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee>
        where TModelUser : class
        where TModelEmployee : class
        where TSearchUser : BaseSearchObject
        where TSearchEmployee : BaseSearchObject
    {
        ServiceResult<TModelUser> Insert(TInsertUser request);
        ServiceResult<TModelEmployee> InsertEmployee(TInsertEmployee request);
        ServiceResult<TModelUser> Update(int id, TUpdateUser request);
        ServiceResult<TModelEmployee> UpdateEmployee(int id, TUpdateEmployee request);
        ServiceResult<string> SoftDelete(int id);
    }
}
