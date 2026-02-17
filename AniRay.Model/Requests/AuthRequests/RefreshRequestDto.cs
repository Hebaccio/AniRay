using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Model.Requests.AuthRequests
{
    public class RefreshRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string AccessToken { get; set; } = string.Empty;
    }
}
