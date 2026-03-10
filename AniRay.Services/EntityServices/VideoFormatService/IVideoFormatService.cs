using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.SearchRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Services.BaseServices.BasicEntitiesService;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.EntityServices.VideoFormatService
{
    public interface IVideoFormatService : IBasicEntitiesService<VideoFormat>
    {
    }
}
