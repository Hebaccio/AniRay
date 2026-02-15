using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Entities
{
    public class BluRayCart
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("BluRayId")]
        public int BluRayId { get; set; }
        public BluRay BluRay { get; set; }

        public int Amount { get; set; }
    }
}
