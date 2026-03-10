using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.HelperServices.CurrentUserService
{
    public interface ICurrentUserService
    {
        int? UserId { get; }
        bool IsAuthenticated { get; }

        bool IsUser();
        bool IsEmployee();
        bool IsBoss();
        bool IsWorker();

        bool IsSelf(int userId);
    }
}
