using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.UpdateRequests;
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
            RegisterRequestMappings();
            RegisterMovieMappings();
            RegisterUserFavoritesMappings();
        }

        private static void RegisterRequestMappings()
        {
            TypeAdapterConfig<Request, RequestUM>.NewConfig()
                .Map(dest => dest.UserFullName, src => src.User.Name + " " + src.User.LastName)
                .Map(dest => dest.UserMail, src => src.User.Email);

            TypeAdapterConfig<Request, RequestEM>.NewConfig()
                .Map(dest => dest.UserFullName, src => src.User.Name + " " + src.User.LastName)
                .Map(dest => dest.UserMail, src => src.User.Email);
        }

        private static void RegisterMovieMappings()
        {
            TypeAdapterConfig<Movie, MovieUM>.NewConfig()
                .Map(dest => dest.Genres, src => src.MovieGenres.Select(mg => mg.Genre.Name).ToList());
        }

        private static void RegisterUserFavoritesMappings()
        {
            TypeAdapterConfig<UserFavoritesUUR, UserFavorites>.NewConfig().IgnoreNonMapped(true);
        }
    }
}
