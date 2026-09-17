using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Content.Configuration
{
    public class ScheduleItemEntityConfiguration : IEntityTypeConfiguration<ScheduleItemEntity>
    {
        public void Configure(EntityTypeBuilder<ScheduleItemEntity> builder)
        {
            builder.Property(s => s.Time).HasMaxLength(100).IsRequired();
            builder.Property(s => s.Title).HasMaxLength(300).IsRequired();
            builder.Property(s => s.Tag).HasMaxLength(100).IsRequired();
            builder.Property(s => s.Detail).HasMaxLength(2000).IsRequired();
            builder.Property(s => s.Location).HasMaxLength(300).IsRequired();
        }
    }
}
