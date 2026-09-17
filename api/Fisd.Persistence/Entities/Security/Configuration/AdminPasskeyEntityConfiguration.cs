using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fisd.Persistence.Entities.Security.Configuration
{
    public class AdminPasskeyEntityConfiguration : IEntityTypeConfiguration<AdminPasskeyEntity>
    {
        public void Configure(EntityTypeBuilder<AdminPasskeyEntity> builder)
        {
            builder.Property(p => p.CredentialId).HasMaxLength(500).IsRequired();
            builder.Property(p => p.PublicKey).IsRequired();
            builder.Property(p => p.DeviceLabel).HasMaxLength(100).IsRequired();
            builder.Property(p => p.Transports).HasMaxLength(200);
            builder.HasIndex(p => p.CredentialId).IsUnique();

            builder.HasOne(p => p.AdminUser)
                .WithMany(u => u.Passkeys)
                .HasForeignKey(p => p.AdminUserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
