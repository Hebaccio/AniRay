using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.InsertRequests
{
    public class OrderUIR
    {
        [MaxLength(20)]
        [Phone]
        public string UserPhone { get; set; }
        public string UserCountry { get; set; }
        public string UserCity { get; set; }
        public string UserZIP { get; set; }
        public string UserAdress { get; set; }
        public string? UserNotes { get; set; }

        public virtual List<OrderBluRayUIR> BluRay { get; set; } = new List<OrderBluRayUIR>();
    }
}
