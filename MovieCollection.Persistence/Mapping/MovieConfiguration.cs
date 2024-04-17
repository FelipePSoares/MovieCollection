using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieCollection.Domain;

namespace MovieCollection.Persistence.Mapping
{
    public class MovieConfiguration : BaseEntityConfiguration<Movie>
    {
        public override void ConfigureEntity(EntityTypeBuilder<Movie> builder)
        {
            builder.ToTable("Movies");

            builder.Property(p => p.Title)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(p => p.Description);
            builder.Property(p => p.ReleaseDate);
            builder.Property(p => p.Duration);

            builder.HasMany(p => p.Genres)
                .WithMany();

            builder.HasMany(p => p.Users)
                .WithMany(u => u.MovieCollection);
        }
    }
}
