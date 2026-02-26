using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AniRay.Services.Helpers
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Request, RequestUM>
                .NewConfig()
                .Map(dest => dest.UserFullName,
                     src => src.User.Name + " " + src.User.LastName)
                .Map(dest => dest.UserMail,
                     src => src.User.Email);
        }
    }
}
