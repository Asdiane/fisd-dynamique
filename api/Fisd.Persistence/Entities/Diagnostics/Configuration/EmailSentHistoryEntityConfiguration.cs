using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Diagnostics.Configuration
{
    public class EmailSentHistoryEntityConfiguration : IEntityTypeConfiguration<EmailSentHistoryEntity>
    {
        public void Configure(EntityTypeBuilder<EmailSentHistoryEntity> builder)
        {
            builder.Property(e => e.Subject).HasMaxLength(300).IsRequired();
            builder.Property(e => e.Body).IsRequired();
            builder.Property(e => e.SentFrom).HasMaxLength(256).IsRequired();
            builder.Property(e => e.SentTo).HasMaxLength(256).IsRequired();
            builder.Property(e => e.ErrorMessage).HasMaxLength(2000);
            builder.HasIndex(e => e.CreatedAt);
        }
    }
}
