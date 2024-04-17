using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MovieCollection.Domain;

namespace MovieCollection.Persistence.Mapping
{
    public class GenreConfiguration : BaseEntityConfiguration<Genre>
    {
        public override void ConfigureEntity(EntityTypeBuilder<Genre> builder)
        {
            builder.ToTable("Genres");

            builder.Property(p => p.Name)
                .HasMaxLength(150)
                .IsRequired();
        }
    }
}
