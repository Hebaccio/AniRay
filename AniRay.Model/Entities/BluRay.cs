using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Entities
{
    public class BluRay
    {
        [Key]
        public int Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }

        [ForeignKey("VideoFormatId")]
        public int VideoFormatId { get; set; }
        public VideoFormat VideoFormat { get; set; }

        [ForeignKey("AudioFormatId")]
        public int AudioFormatId { get; set; }
        public AudioFormat AudioFormat { get; set; }

        [ForeignKey("MovieId")]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int DiscCount { get; set; }
        public int Runtime { get; set; }
        public int InStock { get; set; }
        public string SubtitleLanguage { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }
    }
}