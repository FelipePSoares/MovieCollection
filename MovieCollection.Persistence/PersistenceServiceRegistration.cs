using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieCollection.Application.Contracts.Persistence;
using MovieCollection.Persistence.Repositories;

namespace MovieCollection.Persistence
{
    public static class PersistenceServiceRegistration
    {
        public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
        {
#if DEBUG
            services.AddDbContext<MovieCollectionDatabaseContext>(
                options => options.UseInMemoryDatabase("AppDb"));
#else
            var connectionString = Environment.GetEnvironmentVariable("MovieCollectionDB") ?? string.Empty;

            services.AddDbContext<MovieCollectionDatabaseContext>(
                options => options.UseSqlServer(connectionString));

            services.AddHealthChecks()
                .AddSqlServer(connectionString);
#endif

            services.AddScoped<IUnitOfWork, UnitOfWork>();

            return services;
        }

        public static IApplicationBuilder UseMigration(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<MovieCollectionDatabaseContext>();
                dbContext.Database.Migrate();
            }

            return app;
        }
    }
}
