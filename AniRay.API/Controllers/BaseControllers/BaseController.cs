using AniRay.Model;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Services.Interfaces.BaseInterfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.BaseControllers
{
    public class BaseController<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee> : ControllerBase 
        where TSearchUser : BaseSearchObject
        where TSearchEmployee : BaseSearchObject
    {

        protected IService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee> _service;

        public BaseController(IService<TModelUser, TModelEmployee, TSearchUser, TSearchEmployee> service)
        {
            _service = service;
        }

        [HttpGet("GetPaged/EmployeesOnly")]
        public virtual PagedResult<TModelEmployee> GetPagedEmployees([FromQuery] TSearchEmployee searchObject)
        {
            return _service.GetPagedEmployees(searchObject);
        }

        [HttpGet("GetById/EmployeesOnly/{id}")]
        public virtual TModelEmployee GetByIdEmployees(int id)
        {
            return _service.GetByIdEmployees(id);
        }

        [HttpGet("GetPaged")]
        public virtual PagedResult<TModelUser> GetPaged([FromQuery] TSearchUser searchObject)
        {
            return _service.GetPaged(searchObject);
        }

        [HttpGet("GetById/{id}")]
        public virtual TModelUser GetById(int id)
        {
            return _service.GetById(id);
        }
    }
}
