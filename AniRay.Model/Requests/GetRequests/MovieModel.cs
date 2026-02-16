using AniRay.Model.Entities;
using AniRay.Model.Requests.SearchRequests;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Model.Requests.GetRequests
{
    public class MovieModel
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public int Favorites { get; set; }
        public string Studio { get; set; }
        public string? Director { get; set; }
        public virtual ICollection<MovieGenresModel>? MovieGenres { get; set; } = new List<MovieGenresModel>();
    }
}
