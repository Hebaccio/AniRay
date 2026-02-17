using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Requests.UpdateRequests
{
    public class UserCartUpdateRequest
    {
        public string CartNotes { get; set; }

        public virtual ICollection<BluRayCart> BluRay { get; set; } = new List<BluRayCart>();
    }
}
