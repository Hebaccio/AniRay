using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Interfaces.BaseInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.Interfaces
{
    public interface IMovieService :
        ICRUDService<MovieUM, MovieEM, MovieUSO, MovieESO, MovieIR, MovieIR, MovieUR, MovieUR>
    {
    }
}
