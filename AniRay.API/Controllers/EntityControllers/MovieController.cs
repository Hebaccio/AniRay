using AniRay.API.Controllers.BaseControllers;
using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Model.Requestss.MovieRequests;
using AniRay.Services.EntityServices.MovieService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.EntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : BaseCRUDController<MovieMU, MovieME, MovieSOU, MovieSOE, Movie, MovieIRU, MovieIRE, MovieURU, MovieURE>
    {
        public MovieController(IMovieService Service) : base(Service) { }

        [NonAction]
        public override async Task<ActionResult<MovieMU>> InsertEntityForUsers(MovieIRU request, CancellationToken cancellationToken)
        {
            return await base.InsertEntityForUsers(request, cancellationToken);
        }

        [NonAction]
        public override async Task<ActionResult<MovieMU>> UpdateEntityForUsers(int id, MovieURU request, CancellationToken cancellationToken)
        {
            return await base.UpdateEntityForUsers(id, request, cancellationToken);
        }
    }
}