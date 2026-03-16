namespace AniRay.Model.Requestss.BluRay
{
    public class BluRayIRU
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public int VideoFormatId { get; set; }
        public int AudioFormatId { get; set; }
        public int MovieId { get; set; }
        public int DiscCount { get; set; }
        public int Runtime { get; set; }
        public int InStock { get; set; }
        public string SubtitleLanguage { get; set; }
        public decimal Price { get; set; }
        public bool IsDeleted { get; set; }
    }
}
