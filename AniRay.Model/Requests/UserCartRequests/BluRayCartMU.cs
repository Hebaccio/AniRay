using AniRay.Model.Entities;
using AniRay.Model.Requests.BluRayRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.UserCartRequests
{
    public class BluRayCartMU
    {
        //public int Id { get; set; }
        //public int CartId { get; set; }
        //public int BluRayId { get; set; }
        public BluRayMU BluRay { get; set; }
        public int Amount { get; set; }
    }
}
