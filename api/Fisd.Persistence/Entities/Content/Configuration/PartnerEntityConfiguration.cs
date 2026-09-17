using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Content.Configuration
{
    public class PartnerEntityConfiguration : IEntityTypeConfiguration<PartnerEntity>
    {
        public void Configure(EntityTypeBuilder<PartnerEntity> builder)
        {
            builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(p => p.Name).HasMaxLength(300).IsRequired();
            builder.Property(p => p.Label).HasMaxLength(100);
            builder.Property(p => p.LogoUrl).HasMaxLength(500).IsRequired();
        }
    }
}
