using AniRay.Model.Requests.RequestRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.EntityServices.RequestService
{
    public interface IRequestService : ICRUDService<RequestMU, RequestME, RequestSOU, RequestSOE, RequestIRU, RequestIRE, RequestURU, RequestURE>
    {
    }
}
