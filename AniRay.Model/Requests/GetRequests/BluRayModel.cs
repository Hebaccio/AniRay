using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Requests.GetRequests
{
    public class BluRayModel
    {
        public int MovieId { get; set; }
        public int Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public BasicClassModel VideoFormat { get; set; }
        public BasicClassModel AudioFormat { get; set; }
        public int DiscCount { get; set; }
        public int Runtime { get; set; }
        public int InStock { get; set; }
        public string SubtitleLanguage { get; set; }
        public decimal Price { get; set; }
    }
}
