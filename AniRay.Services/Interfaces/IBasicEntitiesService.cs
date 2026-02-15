using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.Interfaces.BaseInterfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.Interfaces
{
    public interface IBasicEntitiesService : ICRUDService<BasicClassModel, BasicClassSearchObject, BasicClassInsertRequest, BasicClassUpdateRequest>
    {

    }
}
