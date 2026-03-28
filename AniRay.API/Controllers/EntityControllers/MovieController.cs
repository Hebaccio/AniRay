using AniRay.API.Controllers.BaseControllers;
using AniRay.Model.Entities;
using AniRay.Model.Requests.MovieRequests;
using AniRay.Services.EntityServices.MovieService;
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