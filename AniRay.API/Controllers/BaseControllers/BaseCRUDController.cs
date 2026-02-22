using AniRay.Model;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Services.Interfaces.BaseInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.BaseControllers
{
    public class BaseCRUDController<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee, TDbEntity, TInsertUser, TInsertEmployee, TUpdateUser, TUpdateEmployee> :
        BaseController<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee>
        where TModelUser : class
        where TModelEmployee : class
        where TSearchUser : BaseSO
        where TSearchEmployee : BaseSO
        where TDbEntity : class
    {
        protected new ICRUDService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee, TInsertUser, TInsertEmployee, TUpdateUser, TUpdateEmployee> _service;

        public BaseCRUDController(ICRUDService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee, TInsertUser, TInsertEmployee, TUpdateUser, TUpdateEmployee> service) : base(service)
        {
            _service = service;
        }

        [HttpPost("InsertEntity/ForUsers")]
        [Authorize(Roles = "User")]
        public virtual async Task<ActionResult<TModelUser>> InsertEntityForUsers(TInsertUser request, CancellationToken cancellationToken)
        {
            return await _service.InsertEntityForUsers(request, cancellationToken);
        }

        [HttpPost("InsertEntity/ForEmployees")]
        [Authorize(Policy = "Workers")]
        public virtual async Task<ActionResult<TModelEmployee>> InsertEntityForEmployees(TInsertEmployee request, CancellationToken cancellationToken)
        {
            return await _service.InsertEntityForEmployees(request, cancellationToken);
        }

        [HttpPut("UpdateEntity/ForUsers/{id}")]
        [Authorize(Roles = "User")]
        public virtual async Task<ActionResult<TModelUser>> UpdateEntityForUsers(int id, TUpdateUser request, CancellationToken cancellationToken)
        {
            return await _service.UpdateEntityForUsers(id, request, cancellationToken);
        }

        [HttpPut("UpdateEntity/ForEmployees/{id}")]
        [Authorize(Policy = "Workers")]
        public virtual async Task<ActionResult<TModelEmployee>> UpdateEntityForEmployees(int id, TUpdateEmployee request, CancellationToken cancellationToken)
        {
            return await _service.UpdateEntityForEmployees(id, request, cancellationToken);
        }

        [HttpDelete("SoftDelete/{id}")]
        [Authorize]
        public virtual async Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            return await _service.SoftDelete(id, cancellationToken);
        }
    }
}
