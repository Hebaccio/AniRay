using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.GetRequests
{
    public class UserUM
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
        public BaseClassUM UserRole { get; set; }
        public BaseClassUM UserStatus { get; set; }
        public BaseClassUM Gender { get; set; }
    }
}
