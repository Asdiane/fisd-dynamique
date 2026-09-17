using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Content.Configuration
{
    public class SlideEntityConfiguration : IEntityTypeConfiguration<SlideEntity>
    {
        public void Configure(EntityTypeBuilder<SlideEntity> builder)
        {
            builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(s => s.ImageUrl).HasMaxLength(500).IsRequired();
            builder.Property(s => s.VideoUrl).HasMaxLength(500);
            builder.Property(s => s.FocalPoint).HasMaxLength(50);
            builder.Property(s => s.KickerFr).HasMaxLength(200).IsRequired();
            builder.Property(s => s.KickerEn).HasMaxLength(200).IsRequired();
            builder.Property(s => s.TitleFr).HasMaxLength(300).IsRequired();
            builder.Property(s => s.TitleEn).HasMaxLength(300).IsRequired();
            builder.Property(s => s.TextFr).HasMaxLength(2000).IsRequired();
            builder.Property(s => s.TextEn).HasMaxLength(2000).IsRequired();
            builder.Property(s => s.PlaceFr).HasMaxLength(200);
            builder.Property(s => s.PlaceEn).HasMaxLength(200);
        }
    }
}
