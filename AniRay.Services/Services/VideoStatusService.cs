using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Services.Interfaces;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.Services
{
    public class VideoFormatService : BasicEntitiesService<VideoFormat>, IVideoFormatService
    {
        public VideoFormatService(AniRayDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }
    }

}
