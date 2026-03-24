using AniRay.Model.Entities;
using AniRay.Model.Requests.SearchRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requestss.BluRayRequests
{
    public class BluRaySOU : BaseSO
    {
        public int MovieId { get; set; }
    }
}
