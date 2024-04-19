using Microsoft.Extensions.DependencyInjection;
using MovieCollection.Application.Features.AccessControl;

namespace MovieCollection.Application.Features
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IMovieService, MovieService>();
            services.AddScoped<IGenreService, GenreService>();

            return services;
        }
    }
}
