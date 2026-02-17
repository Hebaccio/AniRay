using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Model.Requests.AuthRequests
{
    public class LogoutDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
