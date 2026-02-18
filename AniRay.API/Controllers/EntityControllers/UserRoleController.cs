using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Interfaces.BasicServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.EntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserRoleController : BaseCRUDController<BaseClassUserModel, BaseClassEmployeeModel, BaseClassUserSearchObject, BaseClassEmployeeSearchObject, AudioFormat,
        BaseClassInsertRequest, BaseClassInsertRequest, BaseClassUserUpdateRequest, BaseClassEmployeeUpdateRequest>
    {
        public UserRoleController(IUserRoleService service)
            : base(service)
        {
        }

        [HttpGet("GetPaged/EmployeesOnly")]
        [NonAction]
        public override PagedResult<BaseClassEmployeeModel> GetPagedEmployees([FromQuery] BaseClassEmployeeSearchObject searchObject)
        {
            return _service.GetPagedEmployees(searchObject);
        }

        [HttpGet("GetById/EmployeesOnly/{id}")]
        [NonAction]
        public override BaseClassEmployeeModel GetByIdEmployees(int id)
        {
            return _service.GetByIdEmployees(id);
        }

        [HttpPost("Insert/EmployeesOnly")]
        [NonAction]
        public override ServiceResult<BaseClassEmployeeModel> InsertEmployee(BaseClassInsertRequest request)
        {
            return _service.InsertEmployee(request);
        }

        [HttpPut("Update/EmployeesOnly/{id}")]
        [NonAction]
        public override ServiceResult<BaseClassEmployeeModel> UpdateEmployee(int id, BaseClassEmployeeUpdateRequest request)
        {
            return _service.UpdateEmployee(id, request);
        }

        [HttpDelete("SoftDelete/{id}")]
        [NonAction]
        public override ServiceResult<string> SoftDelete(int id)
        {
            return _service.SoftDelete(id);
        }

        [HttpGet("GetPaged")]
        [NonAction]
        public override PagedResult<BaseClassUserModel> GetPaged([FromQuery] BaseClassUserSearchObject searchObject)
        {
            return _service.GetPaged(searchObject);
        }

        [HttpGet("GetById/{id}")]
        [NonAction]
        public override BaseClassUserModel GetById(int id)
        {
            return _service.GetById(id);
        }

        [HttpPost("Insert")]
        [NonAction]
        public override ServiceResult<BaseClassUserModel> Insert(BaseClassInsertRequest request)
        {
            return _service.Insert(request);
        }

        [HttpPut("Update/{id}")]
        [NonAction]
        public override ServiceResult<BaseClassUserModel> Update(int id, BaseClassUserUpdateRequest request)
        {
            return _service.Update(id, request);
        }
    }

}
