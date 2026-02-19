using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.SearchRequests
{
    public class MovieESO : MovieUSO
    {
        public bool? IsDeleted { get; set; }
    }
}
