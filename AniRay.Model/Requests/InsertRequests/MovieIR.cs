using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.InsertRequests
{
    public class MovieIR
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public int Favorites { get; set; }
        public string Studio { get; set; }
        public string? Director { get; set; }
        public virtual List<int>? GenreIds { get; set; }
    }
}
