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
        where TSearchUser : BaseSO
        where TSearchEmployee : BaseSO
    {
        ServiceResult<TModelUser> InsertEntityForUsers(TInsertUser request);
        ServiceResult<TModelEmployee> InsertEntityForEmployees(TInsertEmployee request);
        ServiceResult<TModelUser> UpdateEntityForUsers(int id, TUpdateUser request);
        ServiceResult<TModelEmployee> UpdateEntityForEmployees(int id, TUpdateEmployee request);
        ServiceResult<string> SoftDelete(int id);
    }
}
