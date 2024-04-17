using Microsoft.Extensions.DependencyInjection;
using MovieCollection.Application.Features.AccessControl;

namespace MovieCollection.Application.Features
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();

            return services;
        }
    }
}
