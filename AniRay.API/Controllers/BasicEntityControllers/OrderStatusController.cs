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

namespace AniRay.API.Controllers.BasicEntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class OrderStatusController : BaseCRUDController<BaseClassUM, BaseClassEM, BaseClassUSO, BaseClassESO, AudioFormat,
        BaseClassIR, BaseClassIR, BaseClassUUR, BaseClassEUR>
    {
        public OrderStatusController(IOrderStatusService service)
            : base(service)
        {
        }

        [HttpGet("GetPagedEntityForUsers/EmployeesOnly")]
        [Authorize(Policy = "Workers")]
        public override PagedResult<BaseClassEM> GetPagedEmployees([FromQuery] BaseClassESO searchObject)
        {
            return _service.GetPagedEntitiesForEmployees(searchObject);
        }

        [HttpGet("EntityGetByIdForUsers/EmployeesOnly/{id}")]
        [Authorize(Policy = "Workers")]
        public override BaseClassEM GetByIdEmployees(int id)
        {
            return _service.EntityGetByIdForEmployees(id);
        }

        [HttpPost("InsertEntityForUsers/EmployeesOnly")]
        [Authorize(Policy = "Workers")]
        public override ServiceResult<BaseClassEM> InsertEmployee(BaseClassIR request)
        {
            return _service.InsertEntityForEmployees(request);
        }

        [HttpPut("UpdateEntityForUsers/EmployeesOnly/{id}")]
        [Authorize(Policy = "Workers")]
        public override ServiceResult<BaseClassEM> UpdateEmployee(int id, BaseClassEUR request)
        {
            return _service.UpdateEntityForEmployees(id, request);
        }

        [HttpDelete("SoftDelete/{id}")]
        [Authorize(Policy = "Workers")]
        public override ServiceResult<string> SoftDelete(int id)
        {
            return _service.SoftDelete(id);
        }

        [HttpGet("GetPagedEntityForUsers")]
        [Authorize(Policy = "Workers")]
        public override PagedResult<BaseClassUM> GetPaged([FromQuery] BaseClassUSO searchObject)
        {
            //Add StateMachine for Normal Users!
            return _service.GetPagedEntityForUsers(searchObject);
        }

        [HttpGet("EntityGetByIdForUsers/{id}")]
        [Authorize(Policy = "Workers")]
        public override BaseClassUM GetById(int id)
        {
            return _service.EntityGetByIdForUsers(id);
        }

        [HttpPost("InsertEntityForUsers")]
        [NonAction]
        public override ServiceResult<BaseClassUM> Insert(BaseClassIR request)
        {
            return _service.InsertEntityForUsers(request);
        }

        [HttpPut("UpdateEntityForUsers/{id}")]
        [NonAction]
        public override ServiceResult<BaseClassUM> Update(int id, BaseClassUUR request)
        {
            return _service.UpdateEntityForUsers(id, request);
        }
    }

}
