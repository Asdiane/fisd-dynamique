using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Content.Configuration
{
    public class SouvenirEntityConfiguration : IEntityTypeConfiguration<SouvenirEntity>
    {
        public void Configure(EntityTypeBuilder<SouvenirEntity> builder)
        {
            builder.Property(s => s.Title).HasMaxLength(300).IsRequired();
            builder.Property(s => s.Status).HasConversion<string>().HasMaxLength(20);
            builder.HasMany(s => s.Photos).WithOne(p => p.Souvenir).HasForeignKey(p => p.SouvenirId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
