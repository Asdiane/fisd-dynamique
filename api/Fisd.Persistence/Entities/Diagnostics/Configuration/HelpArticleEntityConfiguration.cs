using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Diagnostics.Configuration
{
    public class HelpArticleEntityConfiguration : IEntityTypeConfiguration<HelpArticleEntity>
    {
        public void Configure(EntityTypeBuilder<HelpArticleEntity> builder)
        {
            builder.Property(a => a.Category).HasMaxLength(100).IsRequired();
            builder.Property(a => a.TitleFr).HasMaxLength(300).IsRequired();
            builder.Property(a => a.TitleEn).HasMaxLength(300).IsRequired();
            builder.Property(a => a.ContentFr).IsRequired();
            builder.Property(a => a.ContentEn).IsRequired();
        }
    }
}
