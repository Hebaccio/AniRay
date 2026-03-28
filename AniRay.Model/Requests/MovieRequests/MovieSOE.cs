using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.MovieRequests
{
    public class MovieSOE : MovieSOU
    {
        public bool? IsDeleted { get; set; }
    }
}
