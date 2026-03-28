using AniRay.Model.Entities;
using AniRay.Model.Requests.BluRayRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.OrderRequests
{
    public class OrderBluRayMU
    {
        public int OrderId { get; set; }
        public BluRayMU BluRay { get; set; }
        public int Amount { get; set; }
    }
}
