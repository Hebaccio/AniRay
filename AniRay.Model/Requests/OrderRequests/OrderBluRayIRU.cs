using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.OrderRequests
{
    public class OrderBluRayIRU
    {
        public int BluRayId { get; set; }
        public int Amount { get; set; }
    }
}
