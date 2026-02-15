using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Entities
{
    public class User
    {
        [Key]
        public int Id { get; set; }
        public string Pfp { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateOnly Birthday { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool TwoFA { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }

        [ForeignKey("UserRoleId")]
        public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }

        [ForeignKey("UserStatusId")]
        public int UserStatusId { get; set; }
        public UserStatus UserStatus { get; set; }

        [ForeignKey("GenderId")]
        public int GenderId { get; set; }
        public Gender Gender { get; set; }
    }
}
