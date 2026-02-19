using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.UpdateRequests
{
    public class BluRayUR
    {
        public int MovieId { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public int VideoFormatId { get; set; }
        public int AudioFormatId { get; set; }
        public int DiscCount { get; set; }
        public int Runtime { get; set; }
        public int InStock { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }
    }
}
