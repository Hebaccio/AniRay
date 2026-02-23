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
    public class MovieController : BaseCRUDController<MovieUM, MovieEM, MovieUSO, MovieESO, Movie, MovieIR, MovieIR, MovieUR, MovieUR>
    {
        public MovieController(IMovieService Service) : base(Service) { }

        [NonAction]
        public override async Task<ActionResult<MovieUM>> InsertEntityForUsers(MovieIR request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForUsers(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<MovieUM>> UpdateEntityForUsers(int id, MovieUR request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForUsers(id, request, cancellationToken);
        }
    }
}
