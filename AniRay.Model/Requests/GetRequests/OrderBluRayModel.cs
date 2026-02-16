using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Requests.GetRequests
{
    public class OrderBluRayModel
    {
        public int Id { get; set; }
        //public int OrderId { get; set; }
        public BluRayModel BluRay { get; set; }
        public int Amount { get; set; }
    }
}
