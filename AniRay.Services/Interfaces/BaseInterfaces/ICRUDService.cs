using AniRay.Model;
using AniRay.Model.Requests.SearchRequests;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.Interfaces.BaseInterfaces
{
    public interface ICRUDService<TModel, TSearch, TInsert, TUpdate> : IService<TModel, TSearch> where TModel : class where TSearch : BaseSearchObject
    {
        ServiceResult<TModel> Insert(TInsert request);
        ServiceResult<TModel> Update(int id, TUpdate request);
        ServiceResult<string> SoftDelete(int id);
    }
}
