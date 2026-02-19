using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Entities;
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
    public class VideoFormatController : BaseCRUDController<BaseClassUM, BaseClassEM, BaseClassUSO, BaseClassESO, AudioFormat,
        BaseClassIR, BaseClassIR, BaseClassUUR, BaseClassEUR>
    {
        public VideoFormatController(IVideoFormatService service)
            : base(service)
        {
        }

        [HttpGet("GetPaged/EmployeesOnly")]
        [Authorize(Policy = "Workers")]
        public override PagedResult<BaseClassEM> GetPagedEmployees([FromQuery] BaseClassESO searchObject)
        {
            return _service.GetPagedEmployees(searchObject);
        }

        [HttpGet("GetById/EmployeesOnly/{id}")]
        [Authorize(Policy = "Workers")]
        public override BaseClassEM GetByIdEmployees(int id)
        {
            return _service.GetByIdEmployees(id);
        }

        [HttpPost("Insert/EmployeesOnly")]
        [Authorize(Policy = "Workers")]
        public override ServiceResult<BaseClassEM> InsertEmployee(BaseClassIR request)
        {
            return _service.InsertEmployee(request);
        }

        [HttpPut("Update/EmployeesOnly/{id}")]
        [Authorize(Policy = "Workers")]
        public override ServiceResult<BaseClassEM> UpdateEmployee(int id, BaseClassEUR request)
        {
            return _service.UpdateEmployee(id, request);
        }

        [HttpDelete("SoftDelete/{id}")]
        [Authorize(Policy = "Workers")]
        public override ServiceResult<string> SoftDelete(int id)
        {
            return _service.SoftDelete(id);
        }

        [HttpPost("Insert")]
        [NonAction]
        public override ServiceResult<BaseClassUM> Insert(BaseClassIR request)
        {
            return _service.Insert(request);
        }

        [HttpPut("Update/{id}")]
        [NonAction]
        public override ServiceResult<BaseClassUM> Update(int id, BaseClassUUR request)
        {
            return _service.Update(id, request);
        }
    }

}
