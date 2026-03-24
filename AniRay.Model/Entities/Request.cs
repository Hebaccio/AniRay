using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Entities
{
    public class Request
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string? Response { get; set; }
        public DateTime DateTime { get; set; }
        public bool ReadByStaff { get; set; }
        public bool ReadByUser { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
