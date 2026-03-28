using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.RequestRequests
{
    public class RequestIRU
    {
        public string Title { get; set; }
        public string Text { get; set; }
    }
}
