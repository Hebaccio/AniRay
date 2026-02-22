using AniRay.Model;
using AniRay.Model.Requests.SearchRequests;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.Interfaces.BaseInterfaces
{
    public interface IService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee>
        where TSearchUser : BaseSO
        where TSearchEmployee : BaseSO
    {
        public Task<ActionResult<TModelUser>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken);
        public Task<ActionResult<PagedResult<TModelUser>>> GetPagedEntityForUsers(TSearchUser search, CancellationToken cancellationToken);
        public Task<ActionResult<TModelEmployee>> EntityGetByIdForEmployees(int id, CancellationToken cancellationToken);
        public Task<ActionResult<PagedResult<TModelEmployee>>> GetPagedEntityForEmployees(TSearchEmployee search, CancellationToken cancellationToken);
    }
}
