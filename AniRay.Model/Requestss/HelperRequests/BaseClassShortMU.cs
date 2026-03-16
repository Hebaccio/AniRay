using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AniRay.Model.Requestss.HelperRequests
{
    public class BaseClassShortMU
    {
        [JsonIgnore]
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
