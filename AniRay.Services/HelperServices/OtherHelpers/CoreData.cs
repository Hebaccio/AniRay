using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.HelperServices.OtherHelpers
{
    public class CoreData
    {
        public enum CoreOrderStatus
        {
            InProgress = 1,
            Cancelled = 2,
            Rejected = 3,
            Processed = 4,
        }

        public enum CoreUserRole
        {
            User = 1,
            Employee = 2,
            Boss = 3
        }

        public enum CoreUserStatus
        {
            Active = 1,
            Suspended = 2,
            Deleted = 3
        }

        public enum CoreGender
        {
            Male = 1,
            Female = 2
        }
    }
}
