using AniRay.Model;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Services.Interfaces.BaseInterfaces;
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

        [HttpPost("Insert/EmployeesOnly")]
        public virtual ServiceResult<TModelEmployee> InsertEmployee(TInsertEmployee request)
        {
            return _service.InsertEmployee(request);
        }

        [HttpPost("Insert")]
        public virtual ServiceResult<TModelUser> Insert(TInsertUser request)
        {
            return _service.Insert(request);
        }

        [HttpPut("Update/EmployeesOnly/{id}")]
        public virtual ServiceResult<TModelEmployee> UpdateEmployee(int id, TUpdateEmployee request)
        {
            return _service.UpdateEmployee(id, request);
        }

        [HttpPut("Update/{id}")]
        public virtual ServiceResult<TModelUser> Update(int id, TUpdateUser request)
        {
            return _service.Update(id, request);
        }

        [HttpDelete("SoftDelete/{id}")]
        public virtual ServiceResult<string> SoftDelete(int id)
        {
            return _service.SoftDelete(id);
        }
    }

}
