using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Content.Configuration
{
    public class ProgramDayEntityConfiguration : IEntityTypeConfiguration<ProgramDayEntity>
    {
        public void Configure(EntityTypeBuilder<ProgramDayEntity> builder)
        {
            builder.Property(d => d.Label).HasMaxLength(200).IsRequired();
            builder.Property(d => d.DateLabel).HasMaxLength(300).IsRequired();
            builder.HasMany(d => d.Schedule).WithOne(s => s.ProgramDay).HasForeignKey(s => s.ProgramDayId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
