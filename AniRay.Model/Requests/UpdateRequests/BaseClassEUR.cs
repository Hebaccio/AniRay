using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AniRay.Model.Requests.UpdateRequests
{
    public class BaseClassEUR
    {
        [Required(ErrorMessage = "You must provide a name")]
        public string Name { get; set; }
        [Required]
        public bool IsDeleted { get; set; }
    }
}
