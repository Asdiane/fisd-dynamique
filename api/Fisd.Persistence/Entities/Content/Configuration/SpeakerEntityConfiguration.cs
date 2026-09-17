using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Content.Configuration
{
    public class SpeakerEntityConfiguration : IEntityTypeConfiguration<SpeakerEntity>
    {
        public void Configure(EntityTypeBuilder<SpeakerEntity> builder)
        {
            builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(s => s.Name).HasMaxLength(200).IsRequired();
            builder.Property(s => s.RoleFr).HasMaxLength(300).IsRequired();
            builder.Property(s => s.RoleEn).HasMaxLength(300).IsRequired();
            builder.Property(s => s.ImageUrl).HasMaxLength(500).IsRequired();
            builder.Property(s => s.DescriptionFr).HasMaxLength(1000).IsRequired();
            builder.Property(s => s.DescriptionEn).HasMaxLength(1000).IsRequired();
        }
    }
}
