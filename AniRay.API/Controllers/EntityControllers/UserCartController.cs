using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Migrations;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.EntityServices.UserCartService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.EntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserCartController : BaseCRUDController<UserCartUM, UserCartEM, BaseSO, BaseSO, UserCart, UserCartUIR, UserCartEIR, UserCartUUR, UserCartEUR>
    {
        public UserCartController(IUserCartService Service) : base(Service) { }


        [HttpGet("EntityGetById/ForUsers")]
        [Authorize(Roles = "User")]
        public new async Task<ActionResult<UserCartUM>> EntityGetByIdForUsers(CancellationToken cancellationToken)
        {
            return await _service.EntityGetByIdForUsers(null, cancellationToken);
        }

        [HttpPut("UpdateEntity/ForUsers")]
        [Authorize(Roles = "User")]
        public new async Task<ActionResult<UserCartUM>> UpdateEntityForUsers(UserCartUUR request, CancellationToken cancellationToken)
        {
            return await _service.UpdateEntityForUsers(null, request, cancellationToken);
        }

        [HttpGet("EntityGetById/ForUsers/{id}")]
        [NonAction]
        public override async Task<ActionResult<UserCartUM>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            return await _service.EntityGetByIdForUsers(id, cancellationToken);
        }

        [HttpGet("GetPagedEntity/ForUsers")]
        [NonAction]
        public override async Task<ActionResult<PagedResult<UserCartUM>>> GetPagedEntityForUsers([FromQuery] BaseSO searchObject, CancellationToken cancellationToken)
        {
            return await _service.GetPagedEntityForUsers(searchObject, cancellationToken);
        }

        [HttpGet("EntityGetById/ForEmployees/{id}")]
        [Authorize(Policy = "Workers")]
        [NonAction]
        public override async Task<ActionResult<UserCartEM>> EntityGetByIdForEmployees(int id, CancellationToken cancellationToken)
        {
            return await _service.EntityGetByIdForEmployees(id, cancellationToken);
        }

        [HttpGet("GetPagedEntity/ForEmployees")]
        [Authorize(Policy = "Workers")]
        [NonAction]
        public override async Task<ActionResult<PagedResult<UserCartEM>>> GetPagedEntitiesForEmployees([FromQuery] BaseSO searchObject, CancellationToken cancellationToken)
        {
            return await _service.GetPagedEntityForEmployees(searchObject, cancellationToken);
        }

        [HttpPost("InsertEntity/ForUsers")]
        [Authorize(Roles = "User")]
        [NonAction]
        public override async Task<ActionResult<UserCartUM>> InsertEntityForUsers(UserCartUIR request, CancellationToken cancellationToken)
        {
            return await _service.InsertEntityForUsers(request, cancellationToken);
        }

        [HttpPost("InsertEntity/ForEmployees")]
        [Authorize(Policy = "Workers")]
        [NonAction]
        public override async Task<ActionResult<UserCartEM>> InsertEntityForEmployees(UserCartEIR request, CancellationToken cancellationToken)
        {
            return await _service.InsertEntityForEmployees(request, cancellationToken);
        }

        [HttpPut("UpdateEntity/ForUsers/{id}")]
        [Authorize(Roles = "User")]
        [NonAction]
        public override async Task<ActionResult<UserCartUM>> UpdateEntityForUsers(int id, UserCartUUR request, CancellationToken cancellationToken)
        {
            return await _service.UpdateEntityForUsers(id, request, cancellationToken);
        }

        [HttpPut("UpdateEntity/ForEmployees/{id}")]
        [Authorize(Policy = "Workers")]
        [NonAction]
        public override async Task<ActionResult<UserCartEM>> UpdateEntityForEmployees(int id, UserCartEUR request, CancellationToken cancellationToken)
        {
            return await _service.UpdateEntityForEmployees(id, request, cancellationToken);
        }

        [HttpDelete("SoftDelete/{id}")]
        [Authorize(Policy = "Workers")]
        [NonAction]
        public override async Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            return await _service.SoftDelete(id, cancellationToken);
        }
    }
}