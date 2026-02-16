using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AniRay.Model.Requests.UpdateRequests
{
    public class OrderUpdateRequest
    {
        [Required(ErrorMessage = "Order status is required.")]
        public int? OrderStatusId { get; set; }

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string UserNotes { get; set; }
    }
}
