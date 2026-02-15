using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Entities
{
    public class OrderBluRay
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("OrderId")]
        public int OrderId { get; set; }
        public Order Order { get; set; }

        [ForeignKey("BluRayId")]
        public int BluRayId { get; set; }
        public BluRay BluRay { get; set; }

        public int Amount { get; set; }
    }

}
