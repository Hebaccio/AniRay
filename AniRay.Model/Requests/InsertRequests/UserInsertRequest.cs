using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Requests.InsertRequests
{
    public class UserInsertRequest
    {
        [Required(ErrorMessage = "Profile picture is required.")]
        [MaxLength(300, ErrorMessage = "Profile picture URL cannot exceed 300 characters.")]
        public string Pfp { get; set; }

        [Required(ErrorMessage = "Username is required.")]
        [MaxLength(15, ErrorMessage = "Username cannot exceed 15 characters.")]
        public string Username { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(20, ErrorMessage = "First name cannot exceed 20 characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(30, ErrorMessage = "Last name cannot exceed 30 characters.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        [MaxLength(50, ErrorMessage = "Email cannot exceed 50 characters.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [MaxLength(15, ErrorMessage = "Password cannot exceed 15 characters.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Birthday is required.")]
        public DateOnly? Birthday { get; set; }

        public bool TwoFA { get; set; } = false;

        [Required(ErrorMessage = "Gender is required.")]
        public int GenderId { get; set; }
    }
}
