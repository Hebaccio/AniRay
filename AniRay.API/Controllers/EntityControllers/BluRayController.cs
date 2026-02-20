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
    public class BluRayController : BaseCRUDController<BluRayUM, BluRayEM, BluRayUSO, BluRayESO, BluRay, BluRayIR, BluRayIR, BluRayUR, BluRayUR>
    {
        public BluRayController(IBluRayService service)
            : base(service)
        {
        }

        [HttpGet("GetPagedEntityForUsers/EmployeesOnly")]
        [Authorize(Policy = "Workers")]
        public override PagedResult<BluRayEM> GetPagedEmployees([FromQuery] BluRayESO searchObject)
        {
            return _service.GetPagedEntitiesForEmployees(searchObject);
        }

        [HttpGet("EntityGetByIdForUsers/EmployeesOnly/{id}")]
        [Authorize(Policy = "Workers")]
        public override BluRayEM GetByIdEmployees(int id)
        {
            return _service.EntityGetByIdForEmployees(id);
        }

        [HttpPost("InsertEntityForUsers/EmployeesOnly")]
        [Authorize(Policy = "Workers")]
        public override ServiceResult<BluRayEM> InsertEmployee(BluRayIR request)
        {
            return _service.InsertEntityForEmployees(request);
        }

        [HttpPut("UpdateEntityForUsers/EmployeesOnly/{id}")]
        [Authorize(Policy = "Workers")]
        public override ServiceResult<BluRayEM> UpdateEmployee(int id, BluRayUR request)
        {
            return _service.UpdateEntityForEmployees(id, request);
        }

        [HttpDelete("SoftDelete/{id}")]
        [Authorize(Policy = "Workers")]
        public override ServiceResult<string> SoftDelete(int id)
        {
            return _service.SoftDelete(id);
        }

        [HttpPost("InsertEntityForUsers")]
        [NonAction]
        public override ServiceResult<BluRayUM> Insert(BluRayIR request)
        {
            return _service.InsertEntityForUsers(request);
        }

        [HttpPut("UpdateEntityForUsers/{id}")]
        [NonAction]
        public override ServiceResult<BluRayUM> Update(int id, BluRayUR request)
        {
            return _service.UpdateEntityForUsers(id, request);
        }
    }
}
