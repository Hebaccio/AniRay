using AniRay.Model.Requests.SearchRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AniRay.Model.Requestss.BasicEntitiesRequests
{
    public class BaseClassSOE : BaseSO
    {
        public string? NameFTS { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
