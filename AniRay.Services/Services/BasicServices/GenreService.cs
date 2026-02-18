using AniRay.Model.Data;
using AniRay.Model.Entities;
using AniRay.Services.Interfaces.BasicServices;
using MapsterMapper;
using System;
using System.Collections.Generic;
using System.Text;

namespace AniRay.Services.Services.BasicServices
{
    public class GenreService : BasicEntitiesService<Genre>, IGenreService
    {
        public GenreService(AniRayDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }
    }

}
