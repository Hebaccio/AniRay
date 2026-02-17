using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Requests.SearchRequests
{
    public class UserCartSearchObject : BaseSearchObject
    {
        public int UserId { get; set; }
    }
}
