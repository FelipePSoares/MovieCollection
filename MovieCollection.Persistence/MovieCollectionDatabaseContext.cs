using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MovieCollection.Domain.AccessControl;

namespace MovieCollection.Persistence
{
    public class MovieCollectionDatabaseContext(DbContextOptions<MovieCollectionDatabaseContext> options) :
        IdentityDbContext<User, IdentityRole<Guid>, Guid>(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
