using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.InsertRequests
{
    public class UserFavoritesIR
    {
        public int UserId { get; set; }
        public int MovieId { get; set; }
    }
}
