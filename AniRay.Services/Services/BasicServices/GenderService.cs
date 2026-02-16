using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Services.Interfaces;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.Services.BasicServices
{
    public class GenderService : BasicEntitiesService<Gender>, IGenderService
    {
        public GenderService(AniRayDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }
    }

}
