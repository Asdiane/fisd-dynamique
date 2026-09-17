using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Content.Configuration
{
    public class EditionEntityConfiguration : IEntityTypeConfiguration<EditionEntity>
    {
        public void Configure(EntityTypeBuilder<EditionEntity> builder)
        {
            builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(e => e.BadgeFr).HasMaxLength(200).IsRequired();
            builder.Property(e => e.BadgeEn).HasMaxLength(200).IsRequired();
            builder.Property(e => e.TitleFr).HasMaxLength(300).IsRequired();
            builder.Property(e => e.TitleEn).HasMaxLength(300).IsRequired();
            builder.Property(e => e.TextFr).HasMaxLength(2000).IsRequired();
            builder.Property(e => e.TextEn).HasMaxLength(2000).IsRequired();
            builder.Property(e => e.LocationLabelFr).HasMaxLength(300);
            builder.Property(e => e.LocationLabelEn).HasMaxLength(300);
            builder.Property(e => e.TicketingUrl).HasMaxLength(500);
            builder.HasIndex(e => e.Year).IsUnique();
            builder.HasMany(e => e.Days).WithOne(d => d.Edition).HasForeignKey(d => d.EditionId).OnDelete(DeleteBehavior.Cascade);
            builder.HasMany(e => e.Speakers).WithOne(s => s.Edition).HasForeignKey(s => s.EditionId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
