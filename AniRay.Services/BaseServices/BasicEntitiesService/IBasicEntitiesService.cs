using AniRay.Model.Requests.BasicEntitiesRequests;
using AniRay.Services.BaseServices.BaseCRUDService;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.BaseServices.BasicEntitiesService
{
    public interface IBasicEntitiesService<TdbEntity> :
        ICRUDService<
            BaseClassMU, BaseClassME, 
            BaseClassSOU, BaseClassSOE, 
            BaseClassIRU, BaseClassIRE, 
            BaseClassURU, BaseClassURE>
    {

    }
}
