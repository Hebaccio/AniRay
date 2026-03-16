using AniRay.Model.Entities;
using AniRay.Model.Requestss.BluRay;
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
        public BluRayME BluRay { get; set; }
        public int Amount { get; set; }
    }
}
