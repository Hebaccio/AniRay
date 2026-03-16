using AniRay.Model;
using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using AniRay.Services.HelperServices.CurrentUserService;
using MapsterMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Dynamic.Core;
using System.Threading;

namespace AniRay.Services.EntityServices.MovieService
{
    public class MovieService : BaseCRUDService<MovieUM, MovieEM, MovieUSO, MovieESO, Movie, MovieUIR, MovieEIR, MovieUUR, MovieEUR>, IMovieService
    {
        private readonly ICurrentUserService _currentUser;

        public MovieService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUser) : base(context, mapper, currentUser)
        {
            _currentUser = currentUser;
        }

        

    }
}