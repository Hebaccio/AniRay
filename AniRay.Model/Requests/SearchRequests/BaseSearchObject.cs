using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Model.Requests.SearchRequests
{
    public class BaseSearchObject
    {
        public int Page { get; set; } = 0;
        public int PageSize { get; set; } = 10;
    }

}
