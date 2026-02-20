using AniRay.Model;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Services.Interfaces.BaseInterfaces;
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

        [HttpGet("GetPagedEntityForUsers/EmployeesOnly")]
        public virtual PagedResult<TModelEmployee> GetPagedEmployees([FromQuery] TSearchEmployee searchObject)
        {
            return _service.GetPagedEntitiesForEmployees(searchObject);
        }

        [HttpGet("EntityGetByIdForUsers/EmployeesOnly/{id}")]
        public virtual TModelEmployee GetByIdEmployees(int id)
        {
            return _service.EntityGetByIdForEmployees(id);
        }

        [HttpGet("GetPagedEntityForUsers")]
        public virtual PagedResult<TModelUser> GetPaged([FromQuery] TSearchUser searchObject)
        {
            return _service.GetPagedEntityForUsers(searchObject);
        }

        [HttpGet("EntityGetByIdForUsers/{id}")]
        public virtual TModelUser GetById(int id)
        {
            return _service.EntityGetByIdForUsers(id);
        }
    }
}
