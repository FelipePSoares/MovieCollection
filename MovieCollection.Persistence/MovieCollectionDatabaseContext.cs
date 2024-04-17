using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieCollection.Domain.AccessControl;
using MovieCollection.Persistence.Mapping;

namespace MovieCollection.Persistence
{
    public class MovieCollectionDatabaseContext(DbContextOptions<MovieCollectionDatabaseContext> options) :
        IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new GenreConfiguration());
            modelBuilder.ApplyConfiguration(new MovieConfiguration());
            modelBuilder.ApplyConfiguration(new UserMovieConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
