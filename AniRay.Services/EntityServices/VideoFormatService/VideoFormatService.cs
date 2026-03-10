using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Services.BaseServices.BasicEntitiesService;
using AniRay.Services.HelperServices.CurrentUserService;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.EntityServices.VideoFormatService
{
    public class VideoFormatService : BasicEntitiesService<VideoFormat>, IVideoFormatService
    {
        private readonly ICurrentUserService _currentUserService;
        public VideoFormatService(AniRayDbContext context, IMapper mapper, ICurrentUserService currentUserService)
            : base(context, mapper, currentUserService)
        {
            _currentUserService = currentUserService;
        }
    }
}
