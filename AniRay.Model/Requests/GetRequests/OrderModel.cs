using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Requests.GetRequests
{
    public class OrderModel
    {
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal FullPrice { get; set; }
        public BasicClassModel OrderStatus { get; set; }
        public UserOrderModel User { get; set; }
        public string UserName { get; set; }
        public string UserMail { get; set; }
        public string UserPhone { get; set; }
        public string UserCountry { get; set; }
        public string UserCity { get; set; }
        public string UserZIP { get; set; }
        public string UserAdress { get; set; }
        public string UserNotes { get; set; }

        public virtual ICollection<OrderBluRayModel> BluRay { get; set; } = new List<OrderBluRayModel>();
    }
}
