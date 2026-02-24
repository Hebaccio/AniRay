using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.UpdateRequests
{
    public class BluRayCartUR
    {
        //public int CartId { get; set; }
        public int BluRayId { get; set; }
        public int Amount { get; set; }
    }
}
