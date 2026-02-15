using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Entities
{
    public class RefreshToken 
    {
        [Key]
        public int Id { get; set; }
        public string Token { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }
        
        public DateTime Expires { get; set; }
        public bool Revoked { get; set; }
        public DateTime Created { get; set; }
        public string? ReplacedByToken { get; set; }
    }
}
