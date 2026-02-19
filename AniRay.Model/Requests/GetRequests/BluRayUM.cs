using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.GetRequests
{
    public class BluRayUM
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public BaseClassUM VideoFormat { get; set; }
        public BaseClassUM AudioFormat { get; set; }
        public int DiscCount { get; set; }
        public int Runtime { get; set; }
        public int InStock { get; set; }
        public string SubtitleLanguage { get; set; }
        public decimal Price { get; set; }
    }
}
