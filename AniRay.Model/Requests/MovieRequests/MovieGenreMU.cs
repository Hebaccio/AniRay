using AniRay.Model.Requests.BasicEntitiesRequests;
using System.Text.Json.Serialization;

namespace AniRay.Model.Requests.MovieRequests
{
    public class MovieGenreMU
    {
        [JsonIgnore]
        public int MovieId { get; set; }
        public BaseClassME Genre { get; set; }
    }
}
