using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Model.AuthRequests
{
    public class Verify2FADto
    {
        public int UserId { get; set; }
        public string Code { get; set; }
    }
}
