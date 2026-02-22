using AniRay.Model;
using AniRay.Model.Requests.SearchRequests;
using Microsoft.AspNetCore.Mvc;
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
        Task<ActionResult<TModelUser>> InsertEntityForUsers(TInsertUser request, CancellationToken cancellationToken);
        Task<ActionResult<TModelEmployee>> InsertEntityForEmployees(TInsertEmployee request, CancellationToken cancellationToken);
        Task<ActionResult<TModelUser>> UpdateEntityForUsers(int id, TUpdateUser request, CancellationToken cancellationToken);
        Task<ActionResult<TModelEmployee>> UpdateEntityForEmployees(int id, TUpdateEmployee request, CancellationToken cancellationToken);
        Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken);
    }
}
