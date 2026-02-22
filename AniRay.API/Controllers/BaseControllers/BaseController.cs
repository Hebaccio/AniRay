using AniRay.Model;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Services.Interfaces.BaseInterfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.BaseControllers
{
    public class BaseController<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee> : ControllerBase 
        where TSearchUser : BaseSO
        where TSearchEmployee : BaseSO
    {

        protected IService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee> _service;

        public BaseController(IService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee> service)
        {
            _service = service;
        }

        [HttpGet("EntityGetById/ForUsers/{id}")]
        public virtual async Task<ActionResult<TModelUser>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            return await _service.EntityGetByIdForUsers(id, cancellationToken);
        }

        [HttpGet("GetPagedEntity/ForUsers")]
        public virtual async Task<ActionResult<PagedResult<TModelUser>>> GetPagedEntityForUsers([FromQuery] TSearchUser searchObject, CancellationToken cancellationToken)
        {
            return await _service.GetPagedEntityForUsers(searchObject, cancellationToken);
        }

        [HttpGet("EntityGetById/ForEmployees/{id}")]
        [Authorize(Policy = "Workers")]
        public virtual async Task<ActionResult<TModelEmployee>> EntityGetByIdForEmployees(int id, CancellationToken cancellationToken)
        {
            return await _service.EntityGetByIdForEmployees(id, cancellationToken);
        }

        [HttpGet("GetPagedEntity/ForEmployees")]
        [Authorize(Policy = "Workers")]
        public virtual async Task<ActionResult<PagedResult<TModelEmployee>>> GetPagedEntitiesForEmployees([FromQuery] TSearchEmployee searchObject, CancellationToken cancellationToken)
        {
            return await _service.GetPagedEntityForEmployees(searchObject, cancellationToken);
        }
    }
}
