using AniRay.Model.Requests.HelperRequests;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.BluRayRequests
{
    public class BluRaySOE : BaseSO
    {
        public int MovieId { get; set; }
    }
}
