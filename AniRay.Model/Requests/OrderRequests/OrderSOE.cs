using AniRay.Model.Requests.HelperRequests;

namespace AniRay.Model.Requests.OrderRequests
{
    public class OrderSOE : BaseSO
    {
        public DateTime? DateTimeGTE { get; set; }
        public DateTime? DateTimeLTE { get; set; }
        public decimal? FullPriceGTE { get; set; }
        public decimal? FullPriceLTE { get; set; }
        public int? OrderStatusId { get; set; }
        public int? UserId { get; set; }
        public string? UserNameFTS { get; set; }
        public string? UserMailFTS { get; set; }
        public string? UserCountryFTS { get; set; }
        public string? UserCityFTS { get; set; }
        public string? UserZIPFTS { get; set; }
        public OrderSortField? OrderBy { get; set; }
        public SortTypeEnum.SortType? SortType { get; set; }
    }

    public enum OrderSortField
    {
        DateTime,
        FullPrice
    }
}
