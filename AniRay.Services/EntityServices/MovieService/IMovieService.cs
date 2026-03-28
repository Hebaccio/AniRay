using AniRay.Model.Entities;
using AniRay.Model.Requests.MovieRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.EntityServices.MovieService
{
    public interface IMovieService :
        ICRUDService<MovieMU, MovieME, MovieSOU, MovieSOE, MovieIRU, MovieIRE, MovieURU, MovieURE>
    {
    }
}
