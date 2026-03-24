using AniRay.Model.Entities;
using AniRay.Model.Requestss.HelperRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requestss.UserRequests
{
    public class UserME
    {
        public int Id { get; set; }
        public string Pfp { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateOnly Birthday { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool TwoFA { get; set; }
        public BaseClassShortMU UserRole { get; set; }
        public BaseClassShortMU UserStatus { get; set; }
        public BaseClassShortMU Gender { get; set; }
    }
}
