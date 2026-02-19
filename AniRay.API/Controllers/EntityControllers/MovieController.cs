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
    public class MovieController : BaseCRUDController<MovieUM, MovieEM, MovieUSO, MovieESO, Movie, MovieIR, MovieIR, MovieUR, MovieUR>
    {
        public MovieController(IMovieService service)
            : base(service)
        {
        }

        [HttpGet("GetPaged/EmployeesOnly")]
        [Authorize(Policy = "Workers")]
        public override PagedResult<MovieEM> GetPagedEmployees([FromQuery] MovieESO searchObject)
        {
            return _service.GetPagedEmployees(searchObject);
        }

        [HttpGet("GetById/EmployeesOnly/{id}")]
        [Authorize(Policy = "Workers")]
        public override MovieEM GetByIdEmployees(int id)
        {
            return _service.GetByIdEmployees(id);
        }

        [HttpPost("Insert/EmployeesOnly")]
        [Authorize(Policy = "Workers")]
        public override ServiceResult<MovieEM> InsertEmployee(MovieIR request)
        {
            return _service.InsertEmployee(request);
        }

        [HttpPut("Update/EmployeesOnly/{id}")]
        [Authorize(Policy = "Workers")]
        public override ServiceResult<MovieEM> UpdateEmployee(int id, MovieUR request)
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
        public override ServiceResult<MovieUM> Insert(MovieIR request)
        {
            return _service.Insert(request);
        }

        [HttpPut("Update/{id}")]
        [NonAction]
        public override ServiceResult<MovieUM> Update(int id, MovieUR request)
        {
            return _service.Update(id, request);
        }
    }
}
