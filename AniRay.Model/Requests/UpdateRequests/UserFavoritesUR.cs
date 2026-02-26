using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.UpdateRequests
{
    public class UserFavoritesUR
    {
        //public int UserId { get; set; }
        public List<int>? MovieId { get; set; }
    }
}
