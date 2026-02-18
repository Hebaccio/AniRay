using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Services.Interfaces.BasicServices;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.Services.BasicServices
{
    public class AudioFormatService : BasicEntitiesService<AudioFormat>, IAudioFormatService
    {
        public AudioFormatService(AniRayDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }
    }
}
