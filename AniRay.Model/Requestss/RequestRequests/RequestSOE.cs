using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requestss.HelperRequests;
using AniRay.Model.Requestss.MovieRequests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requestss.RequestRequests
{
    public class RequestSOE : BaseSO
    {
        public string? TitleFTS { get; set; }
        public DateTime? DateTimeGTE { get; set; }
        public DateTime? DateTimeLTE { get; set; }
        public string? UserFullNameFTS { get; set; }
        public string? UserMailFTS { get; set; }
        public RequestSortField? OrderBy { get; set; }
        public SortTypeEnum.SortType? SortType { get; set; }
    }

    public enum RequestSortField
    {
        DateTime,
    }
}
