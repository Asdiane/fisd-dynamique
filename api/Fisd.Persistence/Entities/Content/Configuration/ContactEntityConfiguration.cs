using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Content.Configuration
{
    public class ContactEntityConfiguration : IEntityTypeConfiguration<ContactEntity>
    {
        public void Configure(EntityTypeBuilder<ContactEntity> builder)
        {
            builder.Property(c => c.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(c => c.Type).HasConversion<string>().HasMaxLength(20);
            builder.Property(c => c.Label).HasMaxLength(200).IsRequired();
            builder.Property(c => c.Value).HasMaxLength(300).IsRequired();
        }
    }
}
