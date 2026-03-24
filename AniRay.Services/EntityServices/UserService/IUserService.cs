using AniRay.Model.Entities;
using AniRay.Model.Requestss.UserRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.EntityServices.UserService
{
    public interface IUserService :
        ICRUDService<UserMU, UserME, UserSOU, UserSOE, UserIRU, UserIRE, UserURU, UserURE>
    {
    }
}
