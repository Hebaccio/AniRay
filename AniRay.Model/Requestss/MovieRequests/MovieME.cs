using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requestss.MovieRequests
{
    public class MovieME : MovieMU
    {
        public int Id { get; set; }
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public int Favorites { get; set; }
        public string Studio { get; set; }
        public string? Director { get; set; }
        public virtual List<MovieGenreMU> MovieGenres { get; set; } = new List<MovieGenreMU>();
        public bool IsDeleted { get; set; }
    }
}
