using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requestss.UserRequests
{
    public class UserIRU
    {
        public string Pfp { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateOnly Birthday { get; set; }
        public string Password { get; set; }
        public string Password2 { get; set; }
        public int GenderId { get; set; }
    }
}
