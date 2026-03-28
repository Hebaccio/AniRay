using AniRay.Model.Entities;
using AniRay.Model.Requests.BluRayRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.OrderRequests
{
    public class OrderBluRayME
    {
        public int OrderId { get; set; }
        public BluRayME BluRay { get; set; }
        public int Amount { get; set; }
    }
}
