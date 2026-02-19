using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.GetRequests
{
    public class MovieEM : MovieUM
    {
        public bool IsDeleted { get; set; }
    }
}
