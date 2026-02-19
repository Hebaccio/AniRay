using AniRay.Model.Entities;
using AniRay.Model.Requests.InsertRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.UpdateRequests
{
    public class MovieUR : MovieIR
    {
        public bool IsDeleted { get; set; }
    }
}
