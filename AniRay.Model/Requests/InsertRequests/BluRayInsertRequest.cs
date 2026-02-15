using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Requests.InsertRequests
{
    public class BluRayInsertRequest
    {
        [Required(ErrorMessage = "Image URL is required.")]
        [MaxLength(300, ErrorMessage = "Image URL cannot exceed 300 characters.")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(150, ErrorMessage = "Title cannot exceed 150 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Release date is required.")]
        public DateTime? ReleaseDate { get; set; }

        [Required(ErrorMessage = "Video format is required.")]
        public int? VideoFormatId { get; set; }

        [Required(ErrorMessage = "Audio format is required.")]
        public int? AudioFormatId { get; set; }

        [Required(ErrorMessage = "Movie is required.")]
        public int? MovieId { get; set; }

        [Required(ErrorMessage = "Disc count is required.")]
        [Range(1, 5, ErrorMessage = "Disc count must be between 1 and 5.")]
        public int? DiscCount { get; set; }

        [Required(ErrorMessage = "Runtime is required.")]
        [Range(1, 600, ErrorMessage = "Runtime must be between 1 and 600 minutes.")]
        public int? Runtime { get; set; }

        [Required(ErrorMessage = "Stock is required.")]
        [Range(0, 100, ErrorMessage = "Stock cannot be negative or greater than 100.")]
        public int? InStock { get; set; }

        [Required(ErrorMessage = "Subtitle language is required.")]
        [MaxLength(50, ErrorMessage = "Subtitle language cannot exceed 50 characters.")]
        public string SubtitleLanguage { get; set; }

        [Required(ErrorMessage = "Price is required.")]
        [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
        public decimal? Price { get; set; }
    }
}
