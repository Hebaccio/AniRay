using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Model.Requests.UserCartRequests
{
    public class UserCartIsBluRayInCart
    {
        public int UserCartId { get; set; }
        public int BluRayId { get; set; }
        public double Amount { get; set; }
    }
}
