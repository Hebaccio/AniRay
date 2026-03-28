using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.RequestRequests
{
    public class RequestME
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string? Response { get; set; }
        public DateTime DateTime { get; set; }
        public int UserId { get; set; }
        public bool ReadByStaff { get; set; }
        public string UserFullName { get; set; }
        public string UserMail { get; set; }
    }
}
