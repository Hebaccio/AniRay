using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Requests.GetRequests
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Pfp { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateOnly Birthday { get; set; }
        public bool TwoFA { get; set; }
        public bool IsDeleted { get; set; }

        //public int UserRoleId { get; set; }
        public UserRole UserRole { get; set; }

        //public int UserStatusId { get; set; }
        public UserStatus UserStatus { get; set; }

        //public int GenderId { get; set; }
        public Gender Gender { get; set; }
    }
}
