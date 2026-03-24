using AniRay.Model.Entities;
using AniRay.Model.Requests.GetRequests;
using AniRay.Model.Requests.InsertRequests;
using AniRay.Model.Requests.UpdateRequests;
using AniRay.Model.Requestss.MovieRequests;
using AniRay.Model.Requestss.RequestRequests;
using Mapster;

namespace AniRay.Services.HelperServices.OtherHelpers
{
    public static class MapsterConfig
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig.GlobalSettings.Default.IgnoreNullValues(true);
            RegisterRequestMappings();
            RegisterMovieMappings();
            RegisterUserFavoritesMappings();
            RegisterOrderMappings();
        }

        private static void RegisterRequestMappings()
        {
            TypeAdapterConfig<Request, RequestMU>.NewConfig()
                .Map(dest => dest.UserFullName, src => src.User.Name + " " + src.User.LastName)
                .Map(dest => dest.UserMail, src => src.User.Email);

            TypeAdapterConfig<Request, RequestME>.NewConfig()
                .Map(dest => dest.UserFullName, src => src.User.Name + " " + src.User.LastName)
                .Map(dest => dest.UserMail, src => src.User.Email);
        }

        private static void RegisterMovieMappings()
        {
            TypeAdapterConfig<Movie, MovieMU>.NewConfig()
                .Map(dest => dest.MovieGenres, src => src.MovieGenres.Select(mg => mg.Genre.Name).ToList());
        }

        private static void RegisterUserFavoritesMappings()
        {
            TypeAdapterConfig<UserFavoritesUUR, UserFavorites>.NewConfig().IgnoreNonMapped(true);
        }

        private static void RegisterOrderMappings()
        {
            TypeAdapterConfig<Order, OrderUM>.NewConfig()
            .Map(dest => dest.UserName, src => src.User.Name + " " + src.User.LastName)
            .Map(dest => dest.UserMail, src => src.User.Email);

            TypeAdapterConfig<Order, OrderEM>.NewConfig()
            .Map(dest => dest.UserName, src => src.User.Name + " " + src.User.LastName)
            .Map(dest => dest.UserMail, src => src.User.Email);

            TypeAdapterConfig<OrderUIR, Order>.NewConfig().Ignore(dest => dest.BluRay);
        }
    }
}
