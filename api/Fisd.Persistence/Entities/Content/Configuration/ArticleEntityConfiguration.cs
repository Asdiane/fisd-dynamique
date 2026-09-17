using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Content.Configuration
{
    public class ArticleEntityConfiguration : IEntityTypeConfiguration<ArticleEntity>
    {
        public void Configure(EntityTypeBuilder<ArticleEntity> builder)
        {
            builder.Property(a => a.Title).HasMaxLength(300).IsRequired();
            builder.Property(a => a.Slug).HasMaxLength(300);
            builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(20);
            builder.HasIndex(a => a.Slug).IsUnique().HasFilter("[Slug] IS NOT NULL");
        }
    }
}
