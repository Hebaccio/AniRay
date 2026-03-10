using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.BaseServices.BasicEntitiesService
{
    public interface IBasicEntitiesService<TdbEntity> :
        ICRUDService<
            BaseClassUM, BaseClassEM, 
            BaseClassUSO, BaseClassESO, 
            BaseClassUIR, BaseClassEIR, 
            BaseClassUUR, BaseClassEUR>
    {

    }
}
