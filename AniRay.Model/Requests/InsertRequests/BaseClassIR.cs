using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AniRay.Model.Requests.InsertRequests
{
    public class BaseClassIR
    {
        [Required(ErrorMessage = "You must provide a name")]
        public string Name { get; set; }
    }
}
