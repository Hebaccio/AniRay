using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Requests.SearchRequests
{
    public class UserSearchObject : BaseSearchObject
    {
        public string? UsernameFTS { get; set; }
        public string? FullNameFTS { get; set; }
        public string? EmailFTS { get; set; }
        public DateOnly? BirthdayGTE { get; set; }
        public DateOnly? BirthdayLTE { get; set; }
        public DateTime? CreatedAtGTE { get; set; }
        public DateTime? CreatedAtLTE { get; set; }
        public int? UserRoleId { get; set; }
        public int? UserStatusId { get; set; }
        public int? GenderId { get; set; }
        public UserSortField? OrderBy { get; set; }
        public SortType? SortType { get; set; }
    }

    public enum UserSortField
    {
        Username,
        Name,
        LastName,
        Email,
        Birthday,
        CreatedAt
    }

    public enum SortType
    {
        ascending,
        descending
    }

}
