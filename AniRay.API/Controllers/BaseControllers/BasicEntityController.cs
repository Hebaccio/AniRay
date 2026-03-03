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

namespace AniRay.API.Controllers.BaseControllers
{
    [ApiController]
    [Route("[controller]")]
    public class BasicEntityController<TdbEntity> : BaseCRUDController<BaseClassUM, BaseClassEM, BaseClassUSO, BaseClassESO, TdbEntity,
        BaseClassUIR, BaseClassEIR, BaseClassUUR, BaseClassEUR> where TdbEntity : class
    {
        public BasicEntityController(IBasicEntitiesService<TdbEntity> service)
            : base(service)
        {
        }

        [NonAction]
        public override async Task<ActionResult<BaseClassUM>> InsertEntityForUsers(BaseClassUIR request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForUsers(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<BaseClassUM>> UpdateEntityForUsers(int id, BaseClassUUR request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForUsers(id, request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<BaseClassUM>> EntityGetByIdForUsers(int id, CancellationToken cancellationToken)
        {
            return await base.EntityGetByIdForUsers(id, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<BaseClassEM>> EntityGetByIdForEmployees(int id, CancellationToken cancellationToken)
        {
            return await base.EntityGetByIdForEmployees(id, cancellationToken);
        }

    }
}