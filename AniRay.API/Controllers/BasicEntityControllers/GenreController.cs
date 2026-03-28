using AniRay.API.Controllers.BaseControllers;
using AniRay.Model.Entities;
using AniRay.Services.EntityServices.GenreService;
using Microsoft.AspNetCore.Mvc;

namespace AniRay.API.Controllers.BasicEntityControllers
{
    [ApiController]
    [Route("[controller]")]
    public class GenreController : BasicEntityController<Genre>
    {
        public GenreController(IGenreService service)
            : base(service)
        {
        }

        

    }
}