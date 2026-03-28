using AniRay.Model.Requests.HelperRequests;

namespace AniRay.Model.Requests.RequestRequests
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
