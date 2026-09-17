using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Settings.Configuration
{
    public class SiteSettingsEntityConfiguration : IEntityTypeConfiguration<SiteSettingsEntity>
    {
        public void Configure(EntityTypeBuilder<SiteSettingsEntity> builder)
        {
            builder.Property(s => s.SlideDurationSeconds).IsRequired();
        }
    }
}
