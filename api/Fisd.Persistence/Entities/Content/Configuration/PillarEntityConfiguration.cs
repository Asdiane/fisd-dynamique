using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Content.Configuration
{
    public class PillarEntityConfiguration : IEntityTypeConfiguration<PillarEntity>
    {
        public void Configure(EntityTypeBuilder<PillarEntity> builder)
        {
            builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(p => p.TitleFr).HasMaxLength(300).IsRequired();
            builder.Property(p => p.TitleEn).HasMaxLength(300).IsRequired();
            builder.Property(p => p.TextFr).HasMaxLength(2000).IsRequired();
            builder.Property(p => p.TextEn).HasMaxLength(2000).IsRequired();
            builder.Property(p => p.ImageUrl).HasMaxLength(500).IsRequired();
            builder.Property(p => p.Icon).HasMaxLength(20).IsRequired();
        }
    }
}
