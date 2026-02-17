using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Entities
{
    public class UserCart
    {
        public int Id { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }

        public string CartNotes { get; set; }

        public virtual ICollection<BluRayCart> BluRay { get; set; } = new List<BluRayCart>();
    }
}
