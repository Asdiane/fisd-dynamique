using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Security.Configuration
{
    public class AdminUserEntityConfiguration : IEntityTypeConfiguration<AdminUserEntity>
    {
        public void Configure(EntityTypeBuilder<AdminUserEntity> builder)
        {
            builder.Property(u => u.Email).HasMaxLength(320).IsRequired();
            builder.Property(u => u.Role).HasConversion<string>().HasMaxLength(20);
            builder.Property(u => u.TwoFactorSecret).HasMaxLength(128);
            builder.Property(u => u.IsActive).HasDefaultValue(true);
            builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}
