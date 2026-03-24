using AniRay.Model.Requestss.BasicEntitiesRequests;
using AniRay.Model.Requestss.HelperRequests;
using System.Text.Json.Serialization;

namespace AniRay.Model.Requestss.MovieRequests
{
    public class MovieGenreMU
    {
        [JsonIgnore]
        public int MovieId { get; set; }
        public BaseClassME Genre { get; set; }
    }
}
