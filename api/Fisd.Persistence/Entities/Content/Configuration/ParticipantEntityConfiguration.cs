using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Content.Configuration
{
    public class ParticipantEntityConfiguration : IEntityTypeConfiguration<ParticipantEntity>
    {
        public void Configure(EntityTypeBuilder<ParticipantEntity> builder)
        {
            builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(p => p.NameFr).HasMaxLength(200).IsRequired();
            builder.Property(p => p.NameEn).HasMaxLength(200).IsRequired();
            builder.Property(p => p.DescriptionFr).HasMaxLength(1000).IsRequired();
            builder.Property(p => p.DescriptionEn).HasMaxLength(1000).IsRequired();
        }
    }
}
