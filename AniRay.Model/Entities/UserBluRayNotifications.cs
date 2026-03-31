using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Entities
{
    public class UserBluRayNotifications
    {
        [ForeignKey("UserId")] 
        public int UserId { get; set; }
        public User User { get; set; }

        [ForeignKey("BluRayId")] 
        public int BluRayId { get; set; }
        public BluRay BluRay { get; set; }

        public bool EmailQueued { get; set; } = false;
        public bool EmailSent { get; set; } = false;
    }
}
