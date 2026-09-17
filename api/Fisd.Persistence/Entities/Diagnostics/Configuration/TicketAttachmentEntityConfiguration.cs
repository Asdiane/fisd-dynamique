using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Diagnostics.Configuration
{
    public class TicketAttachmentEntityConfiguration : IEntityTypeConfiguration<TicketAttachmentEntity>
    {
        public void Configure(EntityTypeBuilder<TicketAttachmentEntity> builder)
        {
            builder.Property(a => a.FileName).HasMaxLength(300).IsRequired();
            builder.Property(a => a.OriginalFileName).HasMaxLength(300).IsRequired();
            builder.Property(a => a.ContentType).HasMaxLength(100).IsRequired();
            builder.Property(a => a.StoragePath).HasMaxLength(500).IsRequired();
            builder.Property(a => a.PublicUrl).HasMaxLength(500).IsRequired();
            builder.HasIndex(a => a.TicketId);
        }
    }
}
