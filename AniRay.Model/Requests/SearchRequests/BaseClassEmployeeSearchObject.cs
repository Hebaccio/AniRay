using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AniRay.Model.Requests.SearchRequests
{
    public class BaseClassEmployeeSearchObject : BaseSearchObject
    {
        public string? NameFTS { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
