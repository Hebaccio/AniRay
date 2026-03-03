using AniRay.Model.Entities;
using AniRay.Model.Requests.InsertRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.UpdateRequests
{
    public class MovieEUR
    {
        public string Image { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateOnly ReleaseDate { get; set; }
        public int Favorites { get; set; }
        public string Studio { get; set; }
        public string? Director { get; set; }
        public virtual List<int>? GenreIds { get; set; }
        public bool IsDeleted { get; set; }
    }
}
