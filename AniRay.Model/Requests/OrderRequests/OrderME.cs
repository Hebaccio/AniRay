using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.OrderRequests
{
    public class OrderME
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal FullPrice { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string UserMail { get; set; }
        public string UserPhone { get; set; }
        public string UserCountry { get; set; }
        public string UserCity { get; set; }
        public string UserZIP { get; set; }
        public string UserAdress { get; set; }
        public string UserNotes { get; set; }

        public virtual ICollection<OrderBluRayME> BluRay { get; set; } = new List<OrderBluRayME>();
    }
}
