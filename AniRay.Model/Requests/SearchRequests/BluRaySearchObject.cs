using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Requests.SearchRequests
{
    public class BluRaySearchObject : BaseSearchObject
    {
        public int MovieId { get; set; }
    }
}
