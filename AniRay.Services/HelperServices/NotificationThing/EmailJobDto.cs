using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.HelperServices.NotificationThing
{
    public class EmailJobDto
    {
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;

        public int UserId { get; set; }
        public int BluRayId { get; set; }
    }
}
