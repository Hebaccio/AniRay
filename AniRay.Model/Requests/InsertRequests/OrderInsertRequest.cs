using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Requests.InsertRequests
{
    public class OrderInsertRequest
    {
        [Required(ErrorMessage = "UserId is required.")]
        public int? UserId { get; set; }

        [Required(ErrorMessage = "User phone is required.")]
        [Phone(ErrorMessage = "Invalid phone number format.")]
        [MaxLength(30, ErrorMessage = "User phone cannot exceed 30 characters.")]
        public string UserPhone { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        [MaxLength(100, ErrorMessage = "Country cannot exceed 100 characters.")]
        public string UserCountry { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [MaxLength(100, ErrorMessage = "City cannot exceed 100 characters.")]
        public string UserCity { get; set; }

        [Required(ErrorMessage = "ZIP code is required.")]
        [MaxLength(20, ErrorMessage = "ZIP code cannot exceed 20 characters.")]
        public string UserZIP { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [MaxLength(150, ErrorMessage = "Address cannot exceed 150 characters.")]
        public string UserAdress { get; set; }

        [MaxLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string UserNotes { get; set; }

        [Required(ErrorMessage = "Blu Rays have to be added")]
        [MinLength(1, ErrorMessage = "At least one BluRay must be added.")]
        public List<OrderItemInsertRequest>? BluRayIds { get; set; }
    }

    public class OrderItemInsertRequest
    {
        public int BluRayId { get; set; }
        public int Amount { get; set; }
    }

}
