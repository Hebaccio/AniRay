namespace AniRay.Model.Requests.MovieRequests
{
    public class MovieURE
    {
        public string? Image { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateOnly? ReleaseDate { get; set; }
        public string? Studio { get; set; }
        public string? Director { get; set; }
        public bool? IsDeleted { get; set; }
        public virtual List<int>? GenreIds { get; set; }
    }
}
