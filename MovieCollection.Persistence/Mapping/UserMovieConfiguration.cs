using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieCollection.Domain;

namespace MovieCollection.Persistence.Mapping
{
    public class UserMovieConfiguration : BaseEntityConfiguration<UserMovie>
    {
        public override void ConfigureEntity(EntityTypeBuilder<UserMovie> builder)
        {
            builder.ToTable("UserMovies");

            builder.HasOne(p => p.User)
                .WithMany();

            builder.HasOne(p => p.Movie)
                .WithMany();
        }
    }
}
