using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Model.AuthRequests
{
    public class PasswordChange
    {
        public int UserId { get; set; }
        public string Code { get; set; }
        public string NewPassword { get; set; }
        public string NewRepeatPassword { get; set; }
    }

}
