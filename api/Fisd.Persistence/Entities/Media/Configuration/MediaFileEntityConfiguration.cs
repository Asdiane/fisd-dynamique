using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Media.Configuration
{
    public class MediaFileEntityConfiguration : IEntityTypeConfiguration<MediaFileEntity>
    {
        public void Configure(EntityTypeBuilder<MediaFileEntity> builder)
        {
            builder.Property(m => m.Category).HasMaxLength(50).IsRequired();
            builder.Property(m => m.FileName).HasMaxLength(260).IsRequired();
            builder.Property(m => m.OriginalFileName).HasMaxLength(260).IsRequired();
            builder.Property(m => m.ContentType).HasMaxLength(100).IsRequired();
            builder.Property(m => m.StoragePath).HasMaxLength(500).IsRequired();
            builder.Property(m => m.PublicUrl).HasMaxLength(500).IsRequired();
            builder.Property(m => m.AltText).HasMaxLength(300);
            builder.HasIndex(m => m.Category);
        }
    }
}
