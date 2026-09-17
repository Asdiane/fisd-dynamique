using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Content.Configuration
{
    public class SouvenirPhotoEntityConfiguration : IEntityTypeConfiguration<SouvenirPhotoEntity>
    {
        public void Configure(EntityTypeBuilder<SouvenirPhotoEntity> builder)
        {
            builder.Property(p => p.ImageUrl).HasMaxLength(500).IsRequired();
            builder.Property(p => p.Caption).HasMaxLength(300);
        }
    }
}
