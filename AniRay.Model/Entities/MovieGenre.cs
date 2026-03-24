using System.ComponentModel.DataAnnotations.Schema;

namespace AniRay.Model.Entities
{
    public class MovieGenre
    {
        [ForeignKey("MovieId")]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        [ForeignKey("GenreId")]
        public int GenreId { get; set; }
        public Genre Genre { get; set; }
    }
}
