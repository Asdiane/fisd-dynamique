using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Content.Configuration
{
    public class TestimonialEntityConfiguration : IEntityTypeConfiguration<TestimonialEntity>
    {
        public void Configure(EntityTypeBuilder<TestimonialEntity> builder)
        {
            builder.Property(t => t.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(t => t.AuthorName).HasMaxLength(200).IsRequired();
            builder.Property(t => t.AuthorRoleFr).HasMaxLength(200);
            builder.Property(t => t.AuthorRoleEn).HasMaxLength(200);
            builder.Property(t => t.ContentFr).IsRequired();
            builder.Property(t => t.ContentEn).IsRequired();
        }
    }
}
