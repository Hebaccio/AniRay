using AniRay.Model.Entities;
using AniRay.Model.Requests.MovieRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.UserFavoritesRequests
{
    public class UserFavoritesMU
    {
        public int UserId { get; set; }
        public MovieMU Movie { get; set; }
    }
}
