using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AniRay.Model.Requestss.BasicEntities
{
    public class BaseClassURE
    {
        public string? Name { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
