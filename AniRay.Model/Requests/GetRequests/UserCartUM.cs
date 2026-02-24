using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.GetRequests
{
    public class UserCartUM
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public decimal FullCartPrice { get; set; }
        public string CartNotes { get; set; } = string.Empty;
        public virtual ICollection<BluRayCartUM> BluRay { get; set; } = new List<BluRayCartUM>();
    }
}
