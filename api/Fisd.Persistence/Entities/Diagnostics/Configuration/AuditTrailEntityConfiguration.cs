using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Diagnostics.Configuration
{
    public class AuditTrailEntityConfiguration : IEntityTypeConfiguration<AuditTrailEntity>
    {
        public void Configure(EntityTypeBuilder<AuditTrailEntity> builder)
        {
            builder.ToTable("AuditTrail");
            builder.Property(a => a.TableName).HasMaxLength(200).IsRequired();
            builder.Property(a => a.Action).HasMaxLength(20).IsRequired();
            builder.Property(a => a.KeyValues).HasMaxLength(500);
            builder.HasIndex(a => a.Timestamp);
            builder.HasIndex(a => a.TableName);
        }
    }
}
