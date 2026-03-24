using AniRay.Model.Requests.SearchRequests;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requestss.BluRayRequests
{
    public class BluRaySOE : BaseSO
    {
        public int MovieId { get; set; }
    }
}
