using AniRay.Model.Entities;
using AniRay.Model.Requests.MovieRequests;
using AniRay.Model.Requests.OrderRequests;
using AniRay.Model.Requests.RequestRequests;
using AniRay.Model.Requests.UserFavoritesRequests;
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
            TypeAdapterConfig<UserFavoritesURU, UserFavorites>.NewConfig().IgnoreNonMapped(true);
        }

        private static void RegisterOrderMappings()
        {
            TypeAdapterConfig<Order, OrderMU>.NewConfig()
            .Map(dest => dest.UserName, src => src.User.Name + " " + src.User.LastName)
            .Map(dest => dest.UserMail, src => src.User.Email);

            TypeAdapterConfig<Order, OrderME>.NewConfig()
            .Map(dest => dest.UserName, src => src.User.Name + " " + src.User.LastName)
            .Map(dest => dest.UserMail, src => src.User.Email);

            TypeAdapterConfig<OrderIRU, Order>.NewConfig().Ignore(dest => dest.BluRay);
        }
    }
}
