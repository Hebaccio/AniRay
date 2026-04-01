using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Entities
{
    public class BluRayNotificationTrigger
    {
        [Key]
        public int Key { get; set; }
        [ForeignKey("BluRayId")]
        public int BluRayId { get; set; }
        public BluRay BluRay { get; set; }
        public bool Trigger { get; set; } = false;
    }
}
