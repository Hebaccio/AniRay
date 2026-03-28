using AniRay.Model.Requests.HelperRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AniRay.Model.Requests.BasicEntitiesRequests
{
    public class BaseClassSOE : BaseSO
    {
        public string? NameFTS { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
