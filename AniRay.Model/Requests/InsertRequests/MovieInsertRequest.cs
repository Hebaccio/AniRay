using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AniRay.Model.Requests.InsertRequests
{
    public class MovieInsertRequest
    {
        [Required(ErrorMessage = "Image URL is required.")]
        [MaxLength(500, ErrorMessage = "Image URL cannot exceed 500 characters.")]
        public string Image { get; set; }

        [Required(ErrorMessage = "Title is required.")]
        [MaxLength(200, ErrorMessage = "Title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MaxLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Release date is required.")]
        public DateOnly? ReleaseDate { get; set; }

        [Required(ErrorMessage = "Studio name is required.")]
        [MaxLength(100, ErrorMessage = "Studio name cannot exceed 100 characters.")]
        public string Studio { get; set; }

        [MaxLength(100, ErrorMessage = "Director name cannot exceed 100 characters.")]
        public string? Director { get; set; }

        public List<int>? GenreIds { get; set; }
    }

}
