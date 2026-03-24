using AniRay.Model.Requestss.HelperRequests;

namespace AniRay.Model.Requestss.BluRayRequests
{
    public class BluRayMU
    {
        public int MovieId { get; set; }
        public int Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public BaseClassShortMU VideoFormat { get; set; }
        public BaseClassShortMU AudioFormat { get; set; }
        public int DiscCount { get; set; }
        public int Runtime { get; set; }
        public int InStock { get; set; }
        public string SubtitleLanguage { get; set; }
        public decimal Price { get; set; }
    }
}
