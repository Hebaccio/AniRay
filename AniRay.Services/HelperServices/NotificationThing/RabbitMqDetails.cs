using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.HelperServices.NotificationThing
{
    public class RabbitMqDetails
    {
        public string Host { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public string VirtualHost { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
