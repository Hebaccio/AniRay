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
    public class UserFavoritesME
    {
        public int UserId { get; set; }
        public MovieME Movie { get; set; }
    }
}
