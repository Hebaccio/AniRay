using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.SearchRequests
{
    public class RequestESO : BaseSO
    {
        public string? TitleFTS { get; set; }
        public DateTime? DateTimeGTE { get; set; }
        public DateTime? DateTimeLTE { get; set; }
        public string? UserFullNameFTS { get; set; }
        public string? UserMailFTS { get; set; }
        public RequestSortField? OrderBy { get; set; }
        public SortType? SortType { get; set; }
    }

    public enum RequestSortField
    {
        DateTime,
    }
}
