using AniRay.Model.Entities;
using AniRay.Model.Requestss.HelperRequests;
using AniRay.Model.Requestss.MovieRequests;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.SearchRequests
{
    public class OrderESO : BaseSO
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
