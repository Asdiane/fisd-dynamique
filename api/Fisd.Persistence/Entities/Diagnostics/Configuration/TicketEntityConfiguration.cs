using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Diagnostics.Configuration
{
    public class TicketEntityConfiguration : IEntityTypeConfiguration<TicketEntity>
    {
        public void Configure(EntityTypeBuilder<TicketEntity> builder)
        {
            builder.Property(t => t.Title).HasMaxLength(200).IsRequired();
            builder.Property(t => t.Description).IsRequired();
            builder.Property(t => t.CreatedByEmail).HasMaxLength(256).IsRequired();
            builder.Property(t => t.ResolutionNote).HasMaxLength(2000);
        }
    }
}
