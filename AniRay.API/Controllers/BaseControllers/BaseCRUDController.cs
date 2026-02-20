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

        [HttpPost("InsertEntityForUsers/EmployeesOnly")]
        public virtual ServiceResult<TModelEmployee> InsertEmployee(TInsertEmployee request)
        {
            return _service.InsertEntityForEmployees(request);
        }

        [HttpPost("InsertEntityForUsers")]
        public virtual ServiceResult<TModelUser> Insert(TInsertUser request)
        {
            return _service.InsertEntityForUsers(request);
        }

        [HttpPut("UpdateEntityForUsers/EmployeesOnly/{id}")]
        public virtual ServiceResult<TModelEmployee> UpdateEmployee(int id, TUpdateEmployee request)
        {
            return _service.UpdateEntityForEmployees(id, request);
        }

        [HttpPut("UpdateEntityForUsers/{id}")]
        public virtual ServiceResult<TModelUser> Update(int id, TUpdateUser request)
        {
            return _service.UpdateEntityForUsers(id, request);
        }

        [HttpDelete("SoftDelete/{id}")]
        public virtual ServiceResult<string> SoftDelete(int id)
        {
            return _service.SoftDelete(id);
        }
    }

}
