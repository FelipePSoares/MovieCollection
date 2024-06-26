﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MovieCollection.Application.Contracts.Persistence;
using MovieCollection.Domain.AccessControl;
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

                var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();

                if (userManager.FindByEmailAsync("admin@admin.com").GetAwaiter().GetResult() == null)
                {
                    var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
                    var adminRole = new IdentityRole<Guid>("Administrator");
                    roleManager.CreateAsync(adminRole).GetAwaiter().GetResult();

                    var admin = new User()
                    {
                        UserName = "Admin",
                        Email = "admin@admin.com",
                        FirstName = "Admin",
                        LastName = "Admin",
                        HasIncompletedInformation = false
                    };
                    userManager.CreateAsync(admin, "Admin@123456").GetAwaiter().GetResult();
                    userManager.AddToRoleAsync(admin, "Administrator").GetAwaiter().GetResult();
                }
            }

            return app;
        }
    }
}
