using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Entities
{
    public class Order
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public decimal FullPrice { get; set; }

        [ForeignKey("OrderStatusId")]
        public int OrderStatusId { get; set; }
        public OrderStatus OrderStatus { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }

        public string UserName { get; set; }
        public string UserMail { get; set; }
        public string UserPhone { get; set; }
        public string UserZIP { get; set; }
        public string UserAdress { get; set; }
        public string UserNotes { get; set; }
    }
}
