using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.EntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenderController : BaseCRUDController<BasicClassModel, BasicClassSearchObject, BasicClassInsertRequest, BasicClassUpdateRequest>
    {
        public GenderController(IGenderService service)
            : base(service)
        {
        }

        [Authorize(Roles = "Boss")]
        public override ServiceResult<BasicClassModel> Insert(BasicClassInsertRequest request)
        {
            return base.Insert(request);
        }
    }

}
