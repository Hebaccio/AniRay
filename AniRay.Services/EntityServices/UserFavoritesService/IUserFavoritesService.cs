using AniRay.Model.Entities;
using AniRay.Model.Requests.UserFavoritesRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.EntityServices.UserFavoritesService
{
    public interface IUserFavoritesService : 
        ICRUDService<UserFavoritesMU, UserFavoritesME, UserFavoritesSOU, UserFavoritesSOE, UserFavoritesIRU, UserFavoritesIRE, UserFavoritesURU, UserFavoritesURE>
    {
        public Task<ActionResult<UserFavoritesMU>> RemoveMovieFromFavorites(int? id, CancellationToken cancellationToken);
    }
}
