using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.GetRequests
{
    public class OrderBluRayEM
    {
        //public int Id { get; set; }
        public int OrderId { get; set; }
        //public int BluRayId { get; set; }
        public BluRayEM BluRay { get; set; }
        public int Amount { get; set; }
    }
}
