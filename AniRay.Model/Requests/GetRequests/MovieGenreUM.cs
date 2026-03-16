using AniRay.Model.Entities;
using AniRay.Model.Requestss.BasicEntities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.GetRequests
{
    public class MovieGenreUM
    {
        public int MovieId { get; set; }
        public BaseClassMU Genre { get; set; }
    }
}
