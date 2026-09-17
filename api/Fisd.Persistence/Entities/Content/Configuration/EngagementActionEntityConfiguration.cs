using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Content.Configuration
{
    public class EngagementActionEntityConfiguration : IEntityTypeConfiguration<EngagementActionEntity>
    {
        public void Configure(EntityTypeBuilder<EngagementActionEntity> builder)
        {
            builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.Anchor).HasMaxLength(100).IsRequired();
            builder.Property(e => e.TitleFr).HasMaxLength(300).IsRequired();
            builder.Property(e => e.TitleEn).HasMaxLength(300).IsRequired();
            builder.Property(e => e.TextFr).HasMaxLength(1000).IsRequired();
            builder.Property(e => e.TextEn).HasMaxLength(1000).IsRequired();
            builder.Property(e => e.Icon).HasMaxLength(20).IsRequired();
            builder.Property(e => e.ImageUrl).HasMaxLength(500).IsRequired();
            builder.Property(e => e.Link).HasMaxLength(1000).IsRequired();
            builder.Property(e => e.CtaFr).HasMaxLength(200).IsRequired();
            builder.Property(e => e.CtaEn).HasMaxLength(200).IsRequired();
            builder.Property(e => e.DetailFr).HasMaxLength(1000).IsRequired();
            builder.Property(e => e.DetailEn).HasMaxLength(1000).IsRequired();
        }
    }
}
