using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Requests.GetRequests
{
    public class UserCartModel
    {
        public int Id { get; set; }
        public string CartNotes { get; set; }
        public virtual ICollection<OrderBluRayModel> BluRay { get; set; } = new List<OrderBluRayModel>();
    }
}
