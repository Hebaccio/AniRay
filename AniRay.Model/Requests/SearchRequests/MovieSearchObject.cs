using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Model.Requests.SearchRequests
{
    public class MovieSearchObject : BaseSearchObject
    {
        public bool? IsGenresIncluded { get; set; }
        public string? TitleFTS { get; set; }
        public DateOnly? ReleaseDateGTE { get; set; }
        public DateOnly? ReleaseDateLTE { get; set; }
        public int? FavoritesGTE { get; set; }
        public int? FavoritesLTE { get; set; }
        public string? StudioFTS { get; set; }
        public string? DirectorFTS { get; set; }
        public string? OrderBy { get; set; }
        public List<int>? GenreIds { get; set; }
    }
}
