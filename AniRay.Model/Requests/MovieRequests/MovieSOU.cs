using AniRay.Model.Requests.HelperRequests;

namespace AniRay.Model.Requests.MovieRequests
{
    public class MovieSOU : BaseSO
    {
        public string? TitleFTS { get; set; }
        public DateOnly? ReleaseDateGTE { get; set; }
        public DateOnly? ReleaseDateLTE { get; set; }
        public int? FavoritesGTE { get; set; }
        public int? FavoritesLTE { get; set; }
        public string? StudioFTS { get; set; }
        public string? DirectorFTS { get; set; }
        public bool? IsGenresIncluded { get; set; }
        public List<int>? GenreIds { get; set; }
        public MovieSortField? OrderBy { get; set; }
        public SortTypeEnum.SortType? SortType { get; set; }
    }

    public enum MovieSortField
    {
        Title,
        ReleaseDate,
        Favorites,
        Studio,
        Director
    }
}
