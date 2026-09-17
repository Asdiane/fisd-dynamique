using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Security.Configuration
{
    public class PasswordResetTokenEntityConfiguration : IEntityTypeConfiguration<PasswordResetTokenEntity>
    {
        public void Configure(EntityTypeBuilder<PasswordResetTokenEntity> builder)
        {
            builder.Property(t => t.TokenHash).HasMaxLength(64).IsRequired();
            builder.HasIndex(t => t.TokenHash).IsUnique();
            builder.HasIndex(t => t.AdminUserId);
            builder.HasOne<AdminUserEntity>().WithMany().HasForeignKey(t => t.AdminUserId).OnDelete(DeleteBehavior.Cascade);
        }
    }
}
