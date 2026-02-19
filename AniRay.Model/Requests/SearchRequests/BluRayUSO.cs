using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.SearchRequests
{
    public class BluRayUSO : BaseSO
    {
        public int MovieId { get; set; }
    }
}
