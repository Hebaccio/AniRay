using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.UpdateRequests
{
    public class UserCartUR
    {
        public string CartNotes { get; set; } = string.Empty;
        public virtual ICollection<BluRayCartUR>? BluRay { get; set; } = new List<BluRayCartUR>();
    }
}
