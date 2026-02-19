using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.SearchRequests
{
    public class MovieUSO : BaseSO
    {
        public bool? IsGenresIncluded { get; set; }
        public string? TitleFTS { get; set; }
        public DateOnly? ReleaseDateGTE { get; set; }
        public DateOnly? ReleaseDateLTE { get; set; }
        public int? FavoritesGTE { get; set; }
        public int? FavoritesLTE { get; set; }
        public string? StudioFTS { get; set; }
        public string? DirectorFTS { get; set; }
        public MovieSortField? OrderBy { get; set; }
        public SortType? SortType { get; set; }
        public List<int>? GenreIds { get; set; }
    }

    public enum MovieSortField
    {
        Title,
        ReleaseDate,
        Favorites,
        Studio,
        Director
    }

    public enum SortType
    {
        ascending,
        descending
    }
}
