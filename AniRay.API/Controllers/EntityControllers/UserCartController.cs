using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Migrations;
using AniRay.Model.Requests.HelperRequests;
using AniRay.Model.Requests.UserCartRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using AniRay.Services.EntityServices.UserCartService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.EntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserCartController : BaseCRUDController<UserCartMU, UserCartME, BaseSO, BaseSO, UserCart, UserCartIRU, UserCartIRE, UserCartURU, UserCartURE>
    {
        protected new IUserCartService _service;

        public UserCartController(IUserCartService Service) : base(Service)
        {
            _service = Service;
        }


        [HttpGet("EntityGetById/ForUsers")]
        [Authorize(Roles = "User")]
        public new async Task<ActionResult<UserCartMU>> EntityGetByIdForUsers(CancellationToken cancellationToken)
        {
            return await _service.EntityGetByIdForUsers(null, cancellationToken);
        }

        [HttpPost("AddIndividualBluRayToCart/ForUsers")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<bool>> AddIndividualBluRayToCart(UserCartIndividualURU request, CancellationToken cancellationToken)
        {
            return await _service.AddIndividualBluRayToCart(request, cancellationToken);
        }

        [HttpDelete("RemoveIndividualBluRayFromCart/ForUsers/{id}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<bool>> RemoveIndividualBluRayFromCart(int id, CancellationToken cancellationToken)
        {
            return await _service.RemoveIndividualBluRayFromCart(id, cancellationToken);
        }

        [HttpGet("IsBluRayInCart/ForUsers/{id}")]
        [Authorize(Roles = "User")]
        public async Task<ActionResult<bool>> IsBluRayInCart(int id, CancellationToken cancellationToken)
        {
            return await _service.IsBluRayInCart(id, cancellationToken);
        }

        [HttpPut("UpdateEntity/ForUsers")]
        [Authorize(Roles = "User")]
        public new async Task<ActionResult<UserCartMU>> UpdateEntityForUsers(UserCartURU request, CancellationToken cancellationToken)
        {
            return await _service.UpdateEntityForUsers(null, request, cancellationToken);
        }

        [HttpGet("EntityGetById/ForUsers/{id}")]
        [NonAction]
        public override async Task<ActionResult<UserCartMU>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            return await _service.EntityGetByIdForUsers(id, cancellationToken);
        }

        [HttpGet("GetPagedEntity/ForUsers")]
        [NonAction]
        public override async Task<ActionResult<PagedResult<UserCartMU>>> GetPagedEntityForUsers([FromQuery] BaseSO searchObject, CancellationToken cancellationToken)
        {
            return await _service.GetPagedEntityForUsers(searchObject, cancellationToken);
        }

        [HttpGet("EntityGetById/ForEmployees/{id}")]
        [Authorize(Policy = "Workers")]
        [NonAction]
        public override async Task<ActionResult<UserCartME>> EntityGetByIdForEmployees(int id, CancellationToken cancellationToken)
        {
            return await _service.EntityGetByIdForEmployees(id, cancellationToken);
        }

        [HttpGet("GetPagedEntity/ForEmployees")]
        [Authorize(Policy = "Workers")]
        [NonAction]
        public override async Task<ActionResult<PagedResult<UserCartME>>> GetPagedEntitiesForEmployees([FromQuery] BaseSO searchObject, CancellationToken cancellationToken)
        {
            return await _service.GetPagedEntityForEmployees(searchObject, cancellationToken);
        }

        [HttpPost("InsertEntity/ForUsers")]
        [Authorize(Roles = "User")]
        [NonAction]
        public override async Task<ActionResult<UserCartMU>> InsertEntityForUsers(UserCartIRU request, CancellationToken cancellationToken)
        {
            return await _service.InsertEntityForUsers(request, cancellationToken);
        }

        [HttpPost("InsertEntity/ForEmployees")]
        [Authorize(Policy = "Workers")]
        [NonAction]
        public override async Task<ActionResult<UserCartME>> InsertEntityForEmployees(UserCartIRE request, CancellationToken cancellationToken)
        {
            return await _service.InsertEntityForEmployees(request, cancellationToken);
        }

        [HttpPut("UpdateEntity/ForUsers/{id}")]
        [Authorize(Roles = "User")]
        [NonAction]
        public override async Task<ActionResult<UserCartMU>> UpdateEntityForUsers(int id, UserCartURU request, CancellationToken cancellationToken)
        {
            return await _service.UpdateEntityForUsers(id, request, cancellationToken);
        }

        [HttpPut("UpdateEntity/ForEmployees/{id}")]
        [Authorize(Policy = "Workers")]
        [NonAction]
        public override async Task<ActionResult<UserCartME>> UpdateEntityForEmployees(int id, UserCartURE request, CancellationToken cancellationToken)
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