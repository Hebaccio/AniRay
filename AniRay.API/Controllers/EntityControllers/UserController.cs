using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Entities;
using AniRay.Model.Migrations;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Interfaces;
using AniRay.Services.Interfaces.BasicServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.BasicEntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : BaseCRUDController<UserUM, UserEM, UserESO, UserESO, User, UserIR, UserIR, UserUUR, UserEUR>
    {
        public UserController(IUserService service)
            : base(service)
        {
        }

        /*[HttpGet("GetPaged/EmployeesOnly")]
        [Authorize(Policy = "Workers")]
        public override PagedResult<UserEM> GetPagedEmployees([FromQuery] UserESO searchObject)
        {
            return _service.GetPagedEmployees(searchObject);
        }

        [HttpGet("GetById/EmployeesOnly/{id}")]
        [Authorize(Policy = "Workers")]
        public override UserEM GetByIdEmployees(int id)
        {
            return _service.GetByIdEmployees(id);
        }

        [HttpPost("Insert/EmployeesOnly")]
        [Authorize(Policy = "Workers")]
        public override ServiceResult<UserEM> InsertEmployee(UserIR request)
        {
            return _service.InsertEmployee(request);
        }

        [HttpPut("Update/EmployeesOnly/{id}")]
        [Authorize(Policy = "Workers")]
        public override ServiceResult<UserEM> UpdateEmployee(int id, UserEUR request)
        {
            return _service.UpdateEmployee(id, request);
        }

        [HttpDelete("SoftDelete/{id}")]
        [Authorize(Policy = "Workers")]
        public override ServiceResult<string> SoftDelete(int id)
        {
            return _service.SoftDelete(id);
        }

        [HttpPut("Update/{id}")]
        [Authorize (Roles = "User")]
        public override ServiceResult<UserUM> Update(int id, UserUUR request)
        {
            return _service.Update(id, request);
        }
        
        [HttpPost("Insert")]
        [NonAction]
        public override ServiceResult<UserUM> Insert(UserIR request)
        {
            return _service.Insert(request);
        }

        [HttpGet("GetPaged")]
        [NonAction]
        public override PagedResult<UserUM> GetPaged([FromQuery] UserESO searchObject)
        {
            return base.GetPaged(searchObject);
        }*/
    }
}
