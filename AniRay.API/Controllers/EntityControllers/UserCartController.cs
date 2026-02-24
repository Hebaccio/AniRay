using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Interfaces;
using AniRay.Services.Services.AuthentificationServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.EntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserCartController : BaseCRUDController<UserCartUM, UserCartUM, BaseSO, BaseSO, UserCart, UserCartIR, UserCartIR, UserCartUR, UserCartUR>
    {
        public UserCartController(IUserCartService Service) : base(Service) { }

        [NonAction]
        public override async Task<ActionResult<UserCartUM>> EntityGetByIdForEmployees(int id, CancellationToken cancellationToken)
        {
            return await base.EntityGetByIdForEmployees(id, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserCartUM>> InsertEntityForEmployees(UserCartIR request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForEmployees(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserCartUM>> UpdateEntityForEmployees(int id, UserCartUR request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForEmployees(id, request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<string>> SoftDelete(int id, CancellationToken cancellationToken)
        {
            return await base.SoftDelete(id, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<UserCartUM>> InsertEntityForUsers(UserCartIR request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForUsers(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<PagedResult<UserCartUM>>> GetPagedEntityForUsers([FromQuery] BaseSO searchObject, CancellationToken cancellationToken)
        {
            return await base.GetPagedEntityForUsers(searchObject, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<PagedResult<UserCartUM>>> GetPagedEntitiesForEmployees([FromQuery] BaseSO searchObject, CancellationToken cancellationToken)
        {
            return await base.GetPagedEntitiesForEmployees(searchObject, cancellationToken);
        }

    }
}
