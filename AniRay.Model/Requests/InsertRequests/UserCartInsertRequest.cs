using AniRay.Model.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace AniRay.Model.Requests.InsertRequests
{
    public class UserCartInsertRequest
    {
        public int UserId { get; set; }
    }
}
